import { createContext, useContext, useState, useEffect, type ReactNode } from "react";
import type { AuthUser } from "@/types";
import { privateApi } from "@/lib/axios";

type AuthContextType = {
  user: AuthUser | null;
  login: (userData: AuthUser) => void;
  logout: () => void;
  isAuthenticated: boolean;
  isCompany: boolean;
  isJobSeeker: boolean;
};

const AuthContext = createContext<AuthContextType | null>(null);
const STORAGE_KEY = "skillconnect_user";

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  const [user, setUser] = useState<AuthUser | null>(() => {
    const stored = localStorage.getItem(STORAGE_KEY);
    return stored ? (JSON.parse(stored) as AuthUser) : null;
  });

  // Keep localStorage in sync so a page refresh preserves the session.
  useEffect(() => {
    if (user) {
      localStorage.setItem(STORAGE_KEY, JSON.stringify(user));
      localStorage.setItem("token", user.token);
    } else {
      localStorage.removeItem(STORAGE_KEY);
      localStorage.removeItem("token");
    }
  }, [user]);

  const login = (userData: AuthUser) => {
    setUser(userData);
  };

  const logout = () => {
    // Best-effort server-side logout call; session is cleared client-side
    // regardless of whether this succeeds (stateless JWT).
    privateApi.post("/api/user/logout").catch(() => {});
    setUser(null);
  };

  const isAuthenticated = !!user;
  const isCompany = !!user && user.type === "Company";
  const isJobSeeker = !!user && user.type === "JobSeeker";

  return (
    <AuthContext.Provider
      value={{ user, login, logout, isAuthenticated, isCompany, isJobSeeker }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth must be used within an AuthProvider");
  }
  return context;
};
