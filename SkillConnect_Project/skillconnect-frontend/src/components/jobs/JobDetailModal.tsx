import { useState } from "react";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Textarea } from "@/components/ui/textarea";
import { Label } from "@/components/ui/label";
import { Alert, AlertDescription } from "@/components/ui/alert";
import { Loader2 } from "lucide-react";
import { useAuth } from "@/context/AuthContext";
import { useNavigate } from "react-router-dom";
import { privateApi } from "@/lib/axios";
import type { Job } from "@/types";

type JobDetailModalProps = {
  job: Job | null;
  open: boolean;
  onClose: () => void;
};

export default function JobDetailModal({ job, open, onClose }: JobDetailModalProps) {
  const { isAuthenticated, isJobSeeker } = useAuth();
  const navigate = useNavigate();

  const [coverLetter, setCoverLetter] = useState("");
  const [resumeFile, setResumeFile] = useState<File | null>(null);
  const [applying, setApplying] = useState(false);
  const [message, setMessage] = useState<{ type: "success" | "error"; text: string } | null>(null);

  if (!job) return null;

  const resetAndClose = () => {
    setCoverLetter("");
    setResumeFile(null);
    setMessage(null);
    onClose();
  };

  const handleApply = async () => {
    if (!isAuthenticated) {
      navigate("/login");
      return;
    }
    if (!isJobSeeker) {
      setMessage({ type: "error", text: "Only Job Seekers can apply for jobs." });
      return;
    }

    setApplying(true);
    setMessage(null);
    try {
      const formData = new FormData();
      formData.append("JobId", String(job.id));
      formData.append("CoverLetter", coverLetter);
      if (resumeFile) {
        formData.append("ResumeFile", resumeFile);
      }

      await privateApi.post("/api/jobapplication/apply", formData, {
        headers: { "Content-Type": "multipart/form-data" },
      });

      setMessage({ type: "success", text: `Application submitted for ${job.title}!` });
    } catch (err: any) {
      if (err.response?.status === 409) {
        setMessage({ type: "error", text: "You have already applied to this job." });
      } else {
        setMessage({
          type: "error",
          text: err.response?.data?.message || "Failed to submit application. Please try again.",
        });
      }
    } finally {
      setApplying(false);
    }
  };

  return (
    <Dialog open={open} onOpenChange={resetAndClose}>
      <DialogContent
        className="max-w-2xl max-h-[90vh] overflow-y-auto"
        style={{ backgroundColor: "rgba(255,255,255,1)", opacity: 1, zIndex: 110 }}
      >
        <DialogHeader>
          <DialogTitle className="text-2xl">{job.title}</DialogTitle>
          <DialogDescription className="text-lg font-medium text-gray-700">
            {job.company} • {job.location}
          </DialogDescription>
        </DialogHeader>

        <div className="space-y-6 py-4">
          <div className="flex gap-3">
            <Badge variant="secondary">{job.jobType}</Badge>
            <Badge variant="outline">
              NPR {job.minimumSalary.toLocaleString()} - {job.maximumSalary.toLocaleString()}
            </Badge>
          </div>

          <div>
            <h3 className="font-semibold mb-2">Job Description</h3>
            <p className="text-gray-600 leading-relaxed">
              {job.description || "No detailed description available."}
            </p>
          </div>

          <div className="grid grid-cols-2 gap-4 text-sm">
            <div>
              <strong>Location:</strong> {job.location}
            </div>
            <div>
              <strong>Deadline:</strong> {new Date(job.deadLineDate).toLocaleDateString()}
            </div>
          </div>

          {isJobSeeker && !message?.type.includes("success") && (
            <div className="space-y-3 border-t pt-4">
              <div className="space-y-2">
                <Label htmlFor="coverLetter">Cover Letter (optional)</Label>
                <Textarea
                  id="coverLetter"
                  placeholder="Tell the company why you're a great fit..."
                  value={coverLetter}
                  onChange={(e) => setCoverLetter(e.target.value)}
                  rows={4}
                />
              </div>
              <div className="space-y-2">
                <Label htmlFor="resumeFile">
                  Resume (optional - uses your saved profile resume if left blank)
                </Label>
                <input
                  id="resumeFile"
                  type="file"
                  accept=".pdf,.doc,.docx"
                  onChange={(e) => setResumeFile(e.target.files?.[0] ?? null)}
                  className="block w-full text-sm text-gray-600 file:mr-3 file:rounded-md file:border-0 file:bg-gray-100 file:px-3 file:py-1.5"
                />
              </div>
            </div>
          )}

          {message && (
            <Alert variant={message.type === "error" ? "destructive" : "default"}>
              <AlertDescription>{message.text}</AlertDescription>
            </Alert>
          )}
        </div>

        <div className="flex gap-3 pt-4 border-t">
          <Button variant="outline" className="flex-1" onClick={resetAndClose}>
            Close
          </Button>
          <Button
            className="flex-1"
            onClick={handleApply}
            disabled={applying || message?.type === "success"}
          >
            {applying && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
            {message?.type === "success" ? "Applied" : "Apply Now"}
          </Button>
        </div>
      </DialogContent>
    </Dialog>
  );
}
