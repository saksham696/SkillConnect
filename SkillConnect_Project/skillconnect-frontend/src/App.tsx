import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import { AuthProvider } from "./context/AuthContext";
import Navbar from "./components/layout/Navbar";
import LoginPage from "./pages/auth/LoginPage";
import Register from "./pages/auth/Register";
import LandingPage from "./pages/jobs/LandingPage";
import DashboardPage from "./pages/company/DashboardPage";
import { ProtectedRoute } from "./components/layout/ProtectedRoute";

function App() {
  return (
    <AuthProvider>
      <Router>
        <Navbar />
        <Routes>
          <Route path="/" element={<LandingPage />} />
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<Register />} />

          {/* Company-only routes */}
          <Route
            path="/dashboard"
            element={
              <ProtectedRoute requiredRole="Company">
                <DashboardPage />
              </ProtectedRoute>
            }
          />
        </Routes>
      </Router>
    </AuthProvider>
  );
}

export default App;
