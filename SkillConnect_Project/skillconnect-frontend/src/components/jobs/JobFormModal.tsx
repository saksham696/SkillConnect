import { useState, useEffect } from "react";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import { Alert, AlertDescription } from "@/components/ui/alert";
import { Loader2 } from "lucide-react";
import { privateApi } from "@/lib/axios";
import type { Job } from "@/types";

const JOB_TYPES = ["FullTime", "PartTime", "Contract", "Internship", "Remote"];

type JobFormModalProps = {
  open: boolean;
  job: Job | null; // null = create mode, otherwise edit mode
  onClose: () => void;
  onSaved: () => void;
};

const emptyForm = {
  title: "",
  description: "",
  location: "",
  minimumSalary: "",
  maximumSalary: "",
  jobType: JOB_TYPES[0],
  deadLineDate: "",
  isActive: true,
};

export default function JobFormModal({ open, job, onClose, onSaved }: JobFormModalProps) {
  const [form, setForm] = useState(emptyForm);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");

  useEffect(() => {
    if (job) {
      setForm({
        title: job.title,
        description: job.description,
        location: job.location,
        minimumSalary: String(job.minimumSalary),
        maximumSalary: String(job.maximumSalary),
        jobType: job.jobType,
        deadLineDate: job.deadLineDate?.split("T")[0] ?? "",
        isActive: job.isActive,
      });
    } else {
      setForm(emptyForm);
    }
    setError("");
  }, [job, open]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");

    if (Number(form.minimumSalary) > Number(form.maximumSalary)) {
      setError("Minimum salary cannot be greater than maximum salary.");
      return;
    }

    setSaving(true);
    try {
      const payload = {
        title: form.title,
        description: form.description,
        location: form.location,
        minimumSalary: Number(form.minimumSalary),
        maximumSalary: Number(form.maximumSalary),
        jobType: form.jobType,
        deadLineDate: form.deadLineDate,
        ...(job ? { isActive: form.isActive } : {}),
      };

      if (job) {
        await privateApi.put(`/api/job/update/${job.id}`, payload);
      } else {
        await privateApi.post("/api/job/create", payload);
      }

      onSaved();
    } catch (err: any) {
      setError(err.response?.data?.message || "Failed to save job. Please try again.");
    } finally {
      setSaving(false);
    }
  };

  return (
    <Dialog open={open} onOpenChange={onClose}>
      <DialogContent
        className="max-w-xl max-h-[90vh] overflow-y-auto"
        style={{ backgroundColor: "rgba(255,255,255,1)", opacity: 1, zIndex: 110 }}
      >
        <DialogHeader>
          <DialogTitle>{job ? "Edit Job Posting" : "Post a New Job"}</DialogTitle>
        </DialogHeader>

        <form onSubmit={handleSubmit} className="space-y-4">
          {error && (
            <Alert variant="destructive">
              <AlertDescription>{error}</AlertDescription>
            </Alert>
          )}

          <div className="space-y-2">
            <Label htmlFor="title">Job Title</Label>
            <Input
              id="title"
              value={form.title}
              onChange={(e) => setForm({ ...form, title: e.target.value })}
              required
            />
          </div>

          <div className="space-y-2">
            <Label htmlFor="description">Description</Label>
            <Textarea
              id="description"
              rows={4}
              value={form.description}
              onChange={(e) => setForm({ ...form, description: e.target.value })}
              required
            />
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label htmlFor="location">Location</Label>
              <Input
                id="location"
                value={form.location}
                onChange={(e) => setForm({ ...form, location: e.target.value })}
                required
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="jobType">Job Type</Label>
              <select
                id="jobType"
                className="w-full border rounded-md h-9 px-3 text-sm"
                value={form.jobType}
                onChange={(e) => setForm({ ...form, jobType: e.target.value })}
              >
                {JOB_TYPES.map((t) => (
                  <option key={t} value={t}>
                    {t}
                  </option>
                ))}
              </select>
            </div>
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label htmlFor="minSalary">Minimum Salary (NPR)</Label>
              <Input
                id="minSalary"
                type="number"
                min={0}
                value={form.minimumSalary}
                onChange={(e) => setForm({ ...form, minimumSalary: e.target.value })}
                required
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="maxSalary">Maximum Salary (NPR)</Label>
              <Input
                id="maxSalary"
                type="number"
                min={0}
                value={form.maximumSalary}
                onChange={(e) => setForm({ ...form, maximumSalary: e.target.value })}
                required
              />
            </div>
          </div>

          <div className="space-y-2">
            <Label htmlFor="deadline">Application Deadline</Label>
            <Input
              id="deadline"
              type="date"
              value={form.deadLineDate}
              onChange={(e) => setForm({ ...form, deadLineDate: e.target.value })}
              required
            />
          </div>

          {job && (
            <div className="flex items-center gap-2">
              <input
                id="isActive"
                type="checkbox"
                checked={form.isActive}
                onChange={(e) => setForm({ ...form, isActive: e.target.checked })}
              />
              <Label htmlFor="isActive">Job is active and accepting applications</Label>
            </div>
          )}

          <div className="flex gap-3 pt-2">
            <Button type="button" variant="outline" className="flex-1" onClick={onClose}>
              Cancel
            </Button>
            <Button type="submit" className="flex-1" disabled={saving}>
              {saving && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
              {job ? "Save Changes" : "Post Job"}
            </Button>
          </div>
        </form>
      </DialogContent>
    </Dialog>
  );
}
