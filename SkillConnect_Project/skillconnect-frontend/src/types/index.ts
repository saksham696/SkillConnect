export type UserType = "Company" | "JobSeeker";

export interface AuthUser {
  userId: number;
  email: string;
  name: string;
  type: UserType;
  token: string;
}

export interface Job {
  id: number;
  title: string;
  description: string;
  company: string;
  location: string;
  jobType: string;
  minimumSalary: number;
  maximumSalary: number;
  postedDate: string;
  deadLineDate: string;
  isActive: boolean;
  postedById: number;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface JobApplication {
  id: number;
  jobId: number;
  jobTitle: string;
  companyName: string;
  applicantId: number;
  applicantName: string;
  applicantEmail: string;
  applicationDate: string;
  status: "Pending" | "Accepted" | "Rejected" | string;
  coverLetter: string;
  resumePath: string;
}

export interface CreateJobRequest {
  title: string;
  description: string;
  location: string;
  minimumSalary: number;
  maximumSalary: number;
  jobType: string;
  deadLineDate: string;
}

export interface UpdateJobRequest extends CreateJobRequest {
  isActive: boolean;
}
