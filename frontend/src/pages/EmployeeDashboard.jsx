import { Link, useNavigate } from "react-router-dom";
import "../styles/Dashboard.css";

export default function EmployeeDashboard() {
  const navigate = useNavigate();

  const handleLogout = () => {
    localStorage.removeItem("token");
    localStorage.removeItem("role");
    navigate("/login");
  };

  return (
    <div className="container">
      
      {/* HEADER */}
      <div className="header">
        <h1>Employee Dashboard</h1>

        <button className="logout-btn" onClick={handleLogout}>
          Logout
        </button>
      </div>

      {/* CARDS */}
      <div className="cards">

        <div className="card">
          <h2>Rentals</h2>
          <p>Active and past rentals</p>
          <Link to="/employee/rentals">
            <button>Open</button>
          </Link>
        </div>

        <div className="card">
          <h2>Cars</h2>
          <p>Browse available cars</p>
          <Link to="/">
            <button>Browse</button>
          </Link>
        </div>

      </div>
    </div>
  );
}