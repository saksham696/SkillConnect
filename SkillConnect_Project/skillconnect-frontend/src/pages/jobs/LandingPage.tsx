import { useState, useEffect } from "react";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Search, MapPin, Briefcase, Loader2 } from "lucide-react";
import { publicApi } from "@/lib/axios";
import JobDetailModal from "@/components/jobs/JobDetailModal";
import Pagination from "@/components/layout/Pagination";
import type { Job, PagedResult } from "@/types";

const PAGE_SIZE = 9;

export default function LandingPage() {
  const [result, setResult] = useState<PagedResult<Job> | null>(null);
  const [searchInput, setSearchInput] = useState("");
  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);
  const [loading, setLoading] = useState(false);
  const [selectedJob, setSelectedJob] = useState<Job | null>(null);
  const [isModalOpen, setIsModalOpen] = useState(false);

  useEffect(() => {
    const fetchJobs = async () => {
      try {
        setLoading(true);
        const response = await publicApi.get<PagedResult<Job>>("/api/job/list", {
          params: { page, pageSize: PAGE_SIZE, search: search || undefined },
        });
        setResult(response.data);
      } catch (error) {
        console.error("Error fetching jobs:", error);
      } finally {
        setLoading(false);
      }
    };
    fetchJobs();
  }, [page, search]);

  const handleSearchSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    setPage(1);
    setSearch(searchInput);
  };

  const openJobDetails = (job: Job) => {
    setSelectedJob(job);
    setIsModalOpen(true);
  };

  const jobs = result?.items ?? [];

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Hero Section */}
      <div className="bg-gradient-to-r from-blue-600 to-indigo-600 text-white py-20">
        <div className="max-w-6xl mx-auto text-center px-4">
          <h1 className="text-5xl font-bold mb-4">Find Your Dream Job</h1>
          <p className="text-xl mb-8">
            Discover opportunities that match your skills, powered by Elevate
            Workforce Solutions
          </p>

          <form onSubmit={handleSearchSubmit} className="max-w-md mx-auto relative">
            <Search className="absolute left-4 top-3.5 text-gray-400" />
            <Input
              placeholder="Search jobs, companies, or locations..."
              className="pl-12 py-6 text-lg text-gray-900 bg-white"
              value={searchInput}
              onChange={(e) => setSearchInput(e.target.value)}
            />
          </form>
        </div>
      </div>

      {/* Jobs Section */}
      <div className="max-w-6xl mx-auto px-4 py-12">
        <h2 className="text-3xl font-semibold mb-8">
          {search ? `Results for "${search}"` : "Featured Jobs"}
          {result && (
            <span className="text-base font-normal text-gray-500 ml-3">
              {result.totalCount} job{result.totalCount === 1 ? "" : "s"} found
            </span>
          )}
        </h2>

        {loading && (
          <div className="flex justify-center py-12">
            <Loader2 className="animate-spin w-8 h-8 text-blue-600" />
          </div>
        )}

        {!loading && jobs.length === 0 && (
          <p className="text-gray-500 text-center py-12">
            No jobs found. Try a different search term.
          </p>
        )}

        {!loading && (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {jobs.map((job) => (
              <Card key={job.id} className="hover:shadow-lg transition-shadow flex flex-col">
                <CardHeader>
                  <CardTitle>{job.title}</CardTitle>
                  <CardDescription className="flex items-center gap-2">
                    <Briefcase className="w-4 h-4" /> {job.company}
                  </CardDescription>
                </CardHeader>
                <CardContent className="flex flex-col flex-1">
                  <div className="space-y-3 flex-1">
                    <div className="flex items-center gap-2 text-sm text-gray-600">
                      <MapPin className="w-4 h-4" /> {job.location}
                    </div>

                    <div className="flex gap-2 flex-wrap">
                      <Badge variant="secondary">{job.jobType}</Badge>
                      <Badge variant="outline">
                        NPR {job.minimumSalary.toLocaleString()} -{" "}
                        {job.maximumSalary.toLocaleString()}
                      </Badge>
                    </div>

                    <p className="text-sm text-gray-500">
                      Deadline: {new Date(job.deadLineDate).toLocaleDateString()}
                    </p>
                  </div>

                  <Button className="w-full mt-4" onClick={() => openJobDetails(job)}>
                    View Details
                  </Button>
                </CardContent>
              </Card>
            ))}
          </div>
        )}

        {result && (
          <Pagination
            page={result.page}
            totalPages={result.totalPages}
            hasPreviousPage={result.hasPreviousPage}
            hasNextPage={result.hasNextPage}
            onPageChange={setPage}
          />
        )}
      </div>

      <JobDetailModal
        job={selectedJob}
        open={isModalOpen}
        onClose={() => setIsModalOpen(false)}
      />
    </div>
  );
}
