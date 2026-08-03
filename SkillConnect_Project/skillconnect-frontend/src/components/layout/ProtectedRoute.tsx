import { Navigate } from "react-router-dom";
import { useAuth } from "../../context/AuthContext";

type Props = {
  children: React.ReactNode;
  requiredRole?: "Company" | "JobSeeker";
};

export const ProtectedRoute: React.FC<Props> = ({ children, requiredRole }) => {
  const { isAuthenticated, isCompany, isJobSeeker } = useAuth();

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  if (requiredRole === "Company" && !isCompany) {
    return <Navigate to="/" replace />;
  }

  if (requiredRole === "JobSeeker" && !isJobSeeker) {
    return <Navigate to="/" replace />;
  }

  return <>{children}</>;
};
