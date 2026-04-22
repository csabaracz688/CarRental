import { Link, useNavigate } from "react-router-dom";

export default function CustomerDashboard() {
  const navigate = useNavigate();

  const handleLogout = () => {
    localStorage.removeItem("token");
    localStorage.removeItem("role");
    localStorage.removeItem("userId");
    localStorage.removeItem("userName");
    navigate("/login");
  };

  return (
    <div className="container">
      <div className="header">
        <h1>Customer Dashboard</h1>

        <button className="logout-btn" onClick={handleLogout}>
          Kijelentkezés
        </button>
      </div>

      <div className="cards">
        <div className="card">
          <h2>Autók böngészése</h2>
          <p>Megnézheted az elérhető járműveket és árakat.</p>
          <Link to="/">
            <button>Nyitás</button>
          </Link>
        </div>

        <div className="card">
          <h2>Saját foglalások</h2>
          <p>Aktív és korábbi bérléseid áttekintése.</p>
          <Link to="/customer/rentals">
            <button>Megnyitás</button>
          </Link>
        </div>
      </div>
    </div>
  );
}