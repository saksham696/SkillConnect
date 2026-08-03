import axios from "axios";

// Backend API base URL (ASP.NET Core Web API, see SkillConnect.Api/Properties/launchSettings.json)
const baseURL = "http://localhost:5067";

// Public API (no token needed) - browsing jobs, login, register
export const publicApi = axios.create({
  baseURL,
  headers: {
    "Content-Type": "application/json",
  },
});

// Private API (JWT attached automatically) - create/update/delete jobs, applications, profile
export const privateApi = axios.create({
  baseURL,
  headers: {
    "Content-Type": "application/json",
  },
});

privateApi.interceptors.request.use((config) => {
  const token = localStorage.getItem("token");
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

privateApi.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem("token");
      localStorage.removeItem("skillconnect_user");
    }
    return Promise.reject(error);
  },
);
