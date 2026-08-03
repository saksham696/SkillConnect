import { useState, useEffect, useCallback } from "react";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Loader2, Plus, Pencil, Trash2, Users } from "lucide-react";
import { privateApi } from "@/lib/axios";
import type { Job, JobApplication, PagedResult } from "@/types";
import JobFormModal from "@/components/jobs/JobFormModal";
import Pagination from "@/components/layout/Pagination";

type Tab = "jobs" | "applications";
const PAGE_SIZE = 6;

export default function DashboardPage() {
  const [tab, setTab] = useState<Tab>("jobs");

  // My Jobs state
  const [jobsResult, setJobsResult] = useState<PagedResult<Job> | null>(null);
  const [jobsPage, setJobsPage] = useState(1);
  const [loadingJobs, setLoadingJobs] = useState(false);
  const [formOpen, setFormOpen] = useState(false);
  const [editingJob, setEditingJob] = useState<Job | null>(null);

  // Applications state
  const [appsResult, setAppsResult] = useState<PagedResult<JobApplication> | null>(null);
  const [appsPage, setAppsPage] = useState(1);
  const [loadingApps, setLoadingApps] = useState(false);
  const [updatingAppId, setUpdatingAppId] = useState<number | null>(null);

  const fetchJobs = useCallback(async () => {
    setLoadingJobs(true);
    try {
      const res = await privateApi.get<PagedResult<Job>>("/api/job/my-jobs", {
        params: { page: jobsPage, pageSize: PAGE_SIZE },
      });
      setJobsResult(res.data);
    } catch (err) {
      console.error("Failed to load jobs", err);
    } finally {
      setLoadingJobs(false);
    }
  }, [jobsPage]);

  const fetchApplications = useCallback(async () => {
    setLoadingApps(true);
    try {
      const res = await privateApi.get<PagedResult<JobApplication>>(
        "/api/jobapplication/company",
        { params: { page: appsPage, pageSize: PAGE_SIZE } },
      );
      setAppsResult(res.data);
    } catch (err) {
      console.error("Failed to load applications", err);
    } finally {
      setLoadingApps(false);
    }
  }, [appsPage]);

  useEffect(() => {
    if (tab === "jobs") fetchJobs();
  }, [tab, fetchJobs]);

  useEffect(() => {
    if (tab === "applications") fetchApplications();
  }, [tab, fetchApplications]);

  const openCreate = () => {
    setEditingJob(null);
    setFormOpen(true);
  };

  const openEdit = (job: Job) => {
    setEditingJob(job);
    setFormOpen(true);
  };

  const handleSaved = () => {
    setFormOpen(false);
    fetchJobs();
  };

  const handleDelete = async (job: Job) => {
    if (!confirm(`Delete "${job.title}"? This cannot be undone.`)) return;
    try {
      await privateApi.delete(`/api/job/delete/${job.id}`);
      fetchJobs();
    } catch (err) {
      alert("Failed to delete job.");
    }
  };

  const handleStatusChange = async (app: JobApplication, status: string) => {
    setUpdatingAppId(app.id);
    try {
      await privateApi.put(`/api/jobapplication/${app.id}/status`, { status });
      fetchApplications();
    } catch (err) {
      alert("Failed to update application status.");
    } finally {
      setUpdatingAppId(null);
    }
  };

  const statusColor = (status: string) => {
    if (status === "Accepted") return "bg-green-100 text-green-800 border-green-300";
    if (status === "Rejected") return "bg-red-100 text-red-800 border-red-300";
    return "bg-yellow-100 text-yellow-800 border-yellow-300";
  };

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="max-w-6xl mx-auto px-4 py-10">
        <div className="flex items-center justify-between mb-8">
          <div>
            <h1 className="text-3xl font-bold">Company Dashboard</h1>
            <p className="text-gray-500">Manage your job postings and review applicants.</p>
          </div>
          {tab === "jobs" && (
            <Button onClick={openCreate} className="gap-2">
              <Plus className="w-4 h-4" /> Post a Job
            </Button>
          )}
        </div>

        {/* Tabs */}
        <div className="flex gap-2 border-b mb-8">
          <button
            onClick={() => setTab("jobs")}
            className={`px-4 py-2 text-sm font-medium border-b-2 -mb-px ${
              tab === "jobs"
                ? "border-blue-600 text-blue-600"
                : "border-transparent text-gray-500 hover:text-gray-700"
            }`}
          >
            My Jobs
          </button>
          <button
            onClick={() => setTab("applications")}
            className={`px-4 py-2 text-sm font-medium border-b-2 -mb-px ${
              tab === "applications"
                ? "border-blue-600 text-blue-600"
                : "border-transparent text-gray-500 hover:text-gray-700"
            }`}
          >
            Applications
          </button>
        </div>

        {/* My Jobs Tab */}
        {tab === "jobs" && (
          <>
            {loadingJobs && (
              <div className="flex justify-center py-12">
                <Loader2 className="animate-spin w-8 h-8 text-blue-600" />
              </div>
            )}

            {!loadingJobs && jobsResult?.items.length === 0 && (
              <p className="text-gray-500 text-center py-12">
                You haven't posted any jobs yet. Click "Post a Job" to get started.
              </p>
            )}

            {!loadingJobs && (
              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                {jobsResult?.items.map((job) => (
                  <Card key={job.id}>
                    <CardHeader>
                      <div className="flex items-start justify-between">
                        <div>
                          <CardTitle>{job.title}</CardTitle>
                          <CardDescription>{job.location}</CardDescription>
                        </div>
                        <Badge variant={job.isActive ? "secondary" : "outline"}>
                          {job.isActive ? "Active" : "Closed"}
                        </Badge>
                      </div>
                    </CardHeader>
                    <CardContent>
                      <div className="flex gap-2 mb-3 flex-wrap">
                        <Badge variant="outline">{job.jobType}</Badge>
                        <Badge variant="outline">
                          NPR {job.minimumSalary.toLocaleString()} -{" "}
                          {job.maximumSalary.toLocaleString()}
                        </Badge>
                      </div>
                      <p className="text-sm text-gray-500 mb-4">
                        Deadline: {new Date(job.deadLineDate).toLocaleDateString()}
                      </p>
                      <div className="flex gap-2">
                        <Button
                          variant="outline"
                          size="sm"
                          className="flex-1 gap-2"
                          onClick={() => openEdit(job)}
                        >
                          <Pencil className="w-4 h-4" /> Edit
                        </Button>
                        <Button
                          variant="outline"
                          size="sm"
                          className="flex-1 gap-2 text-red-600 hover:bg-red-50"
                          onClick={() => handleDelete(job)}
                        >
                          <Trash2 className="w-4 h-4" /> Delete
                        </Button>
                      </div>
                    </CardContent>
                  </Card>
                ))}
              </div>
            )}

            {jobsResult && (
              <Pagination
                page={jobsResult.page}
                totalPages={jobsResult.totalPages}
                hasPreviousPage={jobsResult.hasPreviousPage}
                hasNextPage={jobsResult.hasNextPage}
                onPageChange={setJobsPage}
              />
            )}
          </>
        )}

        {/* Applications Tab */}
        {tab === "applications" && (
          <>
            {loadingApps && (
              <div className="flex justify-center py-12">
                <Loader2 className="animate-spin w-8 h-8 text-blue-600" />
              </div>
            )}

            {!loadingApps && appsResult?.items.length === 0 && (
              <p className="text-gray-500 text-center py-12">
                <Users className="w-8 h-8 mx-auto mb-2 text-gray-300" />
                No applications yet.
              </p>
            )}

            {!loadingApps && appsResult && appsResult.items.length > 0 && (
              <div className="space-y-4">
                {appsResult.items.map((app) => (
                  <Card key={app.id}>
                    <CardContent className="py-4">
                      <div className="flex flex-wrap items-start justify-between gap-4">
                        <div>
                          <p className="font-semibold">{app.applicantName}</p>
                          <p className="text-sm text-gray-500">{app.applicantEmail}</p>
                          <p className="text-sm text-gray-700 mt-1">
                            Applied for <strong>{app.jobTitle}</strong> on{" "}
                            {new Date(app.applicationDate).toLocaleDateString()}
                          </p>
                          {app.coverLetter && (
                            <p className="text-sm text-gray-600 mt-2 max-w-xl">
                              "{app.coverLetter}"
                            </p>
                          )}
                          {app.resumePath && (
                            <a
                              href={`http://localhost:5129/${app.resumePath}`}
                              target="_blank"
                              rel="noreferrer"
                              className="text-sm text-blue-600 hover:underline mt-2 inline-block"
                            >
                              View Resume
                            </a>
                          )}
                        </div>
                        <div className="flex flex-col items-end gap-2">
                          <Badge className={statusColor(app.status)} variant="outline">
                            {app.status}
                          </Badge>
                          <div className="flex gap-2">
                            <Button
                              size="sm"
                              variant="outline"
                              disabled={updatingAppId === app.id || app.status === "Accepted"}
                              onClick={() => handleStatusChange(app, "Accepted")}
                            >
                              Accept
                            </Button>
                            <Button
                              size="sm"
                              variant="outline"
                              className="text-red-600"
                              disabled={updatingAppId === app.id || app.status === "Rejected"}
                              onClick={() => handleStatusChange(app, "Rejected")}
                            >
                              Reject
                            </Button>
                          </div>
                        </div>
                      </div>
                    </CardContent>
                  </Card>
                ))}
              </div>
            )}

            {appsResult && (
              <Pagination
                page={appsResult.page}
                totalPages={appsResult.totalPages}
                hasPreviousPage={appsResult.hasPreviousPage}
                hasNextPage={appsResult.hasNextPage}
                onPageChange={setAppsPage}
              />
            )}
          </>
        )}
      </div>

      <JobFormModal
        open={formOpen}
        job={editingJob}
        onClose={() => setFormOpen(false)}
        onSaved={handleSaved}
      />
    </div>
  );
}
