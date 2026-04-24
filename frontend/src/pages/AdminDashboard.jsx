import { Link, useNavigate } from "react-router-dom";
import "../styles/AdminDashboard.css";

export default function AdminDashboard() {
  const navigate = useNavigate();

  const handleLogout = () => {
    localStorage.removeItem("token");
    localStorage.removeItem("role");
    navigate("/login");
  };

  return (
    <div className="container">
      
      <div className="header">
        <h1>Admin Dashboard</h1>

        <button className="logout-btn" onClick={handleLogout}>
          Kijelentkezés
        </button>
      </div>

      <div className="cards">
        
        <div className="card">
          <h2>Autók</h2>
          <p>Összes autó kezelése</p>
          <Link to="/admin/cars">
            <button>Megnyitás</button>
          </Link>
        </div>

        <div className="card">
          <h2>Új autó</h2>
          <p>Új autó hozzáadása</p>
          <Link to="/admin/add-car">
            <button>Hozzáadás</button>
          </Link>
        </div>

      </div>
    </div>
  );
}