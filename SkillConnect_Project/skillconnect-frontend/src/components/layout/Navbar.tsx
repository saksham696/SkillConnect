import { Link, useNavigate } from "react-router-dom";
import { Button } from "@/components/ui/button";
import { useAuth } from "@/context/AuthContext";
import { Briefcase, LayoutDashboard, LogOut } from "lucide-react";

export default function Navbar() {
  const { isAuthenticated, isCompany, user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate("/");
  };

  return (
    <nav className="bg-white border-b sticky top-0 z-50">
      <div className="max-w-6xl mx-auto px-4 h-16 flex items-center justify-between">
        <Link to="/" className="flex items-center gap-2 font-bold text-xl text-blue-700">
          <Briefcase className="w-6 h-6" />
          Elevate Workforce Solutions
        </Link>

        <div className="flex items-center gap-3">
          {isAuthenticated && isCompany && (
            <Link to="/dashboard">
              <Button variant="ghost" className="gap-2">
                <LayoutDashboard className="w-4 h-4" />
                Dashboard
              </Button>
            </Link>
          )}

          {isAuthenticated ? (
            <>
              <span className="text-sm text-gray-600 hidden sm:inline">
                Hi, {user?.name}
              </span>
              <Button variant="outline" className="gap-2" onClick={handleLogout}>
                <LogOut className="w-4 h-4" />
                Logout
              </Button>
            </>
          ) : (
            <>
              <Link to="/login">
                <Button variant="ghost">Login</Button>
              </Link>
              <Link to="/register">
                <Button>Register</Button>
              </Link>
            </>
          )}
        </div>
      </div>
    </nav>
  );
}
