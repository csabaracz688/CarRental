import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import "../styles/UserProfile.css";
import { useToast } from "../components/useToast";

export default function Profile() {
  const [user, setUser] = useState({
    postalCode: "",
    city: "",
    address: "",
    phone: "",
  });

  const token = localStorage.getItem("token");
  const navigate = useNavigate();
  const toast = useToast();

  //  userId a tokenből
  const getUserIdFromToken = () => {
    if (!token) return null;

    try {
      const payload = JSON.parse(atob(token.split(".")[1]));
      return payload.nameid || payload.sub; // backendtől függően
    } catch {
      return null;
    }
  };

  const userId = getUserIdFromToken();

  useEffect(() => {
    const fetchUser = async () => {
      try {
        const res = await fetch(`https://localhost:7077/api/users/${userId}`, {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });

        if (!res.ok) {
          const text = await res.text();
          throw new Error(`Error: ${res.status} - ${text}`);
        }

        const data = await res.json();

        setUser({
          postalCode: data.postalCode ?? "",
          city: data.city ?? "",
          address: data.address ?? "",
          phone: data.phone ?? "",
        });
      } catch (err) {
        console.error("Fetch failed:", err);
      }
    };

    if (userId && token) {
      fetchUser();
    }
  }, [userId, token]);

  const handleChange = (e) => {
    setUser({ ...user, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    try {
      const res = await fetch(
        `https://localhost:7077/api/users/${userId}`,
        {
          method: "PUT",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
          },
          body: JSON.stringify({
            postalCode: Number(user.postalCode),
            city: user.city,
            address: user.address,
            phone: user.phone,
          }),
        }
      );

      if (!res.ok) {
        const text = await res.text();
        throw new Error(`Error: ${res.status} - ${text}`);
      }

      toast("Profile updated!", { type: "success" });
    } catch (err) {
      console.error("Update failed:", err);
      toast("Update failed!", { type: "error" });
    }
  };

  return (
    <div className="profile-page page-shell">
      <button onClick={() => navigate("/")} className="back-btn">
        ← Back
      </button>

      <div className="page-head">
        <div>
          <h1 className="page-title">Profile</h1>
          <p className="page-subtitle">Update your contact and address information.</p>
        </div>
      </div>

      <form className="profile-form card-surface" onSubmit={handleSubmit}>
        <input
          type="number"
          name="postalCode"
          placeholder="Postal Code"
          value={user.postalCode}
          onChange={handleChange}
        />

        <input
          type="text"
          name="city"
          placeholder="City"
          value={user.city}
          onChange={handleChange}
        />

        <input
          type="text"
          name="address"
          placeholder="Street / Address"
          value={user.address}
          onChange={handleChange}
        />

        <input
          type="text"
          name="phone"
          placeholder="Phone"
          value={user.phone}
          onChange={handleChange}
        />

        <button className="btn-primary" type="submit">Save</button>
      </form>
    </div>
  );
}