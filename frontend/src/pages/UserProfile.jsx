import { useEffect, useState } from "react";

export default function Profile() {
  const [user, setUser] = useState({
    postalCode: "",
    city: "",
    address: "",
    phone: "",
  });

  const token = localStorage.getItem("token");
const payload = JSON.parse(atob(token.split('.')[1]));
console.log(payload);

  // 👉 userId a tokenből
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

      alert("Profile updated!");
    } catch (err) {
      console.error("Update failed:", err);
      alert("Update failed!");
    }
  };

  return (
    <div>
      <h1>Profile</h1>

      <form onSubmit={handleSubmit}>
        <input
          type="number"
          name="postalCode"
          placeholder="Postal Code"
          value={user.postalCode}
          onChange={handleChange}
        />
        <br />

        <input
          type="text"
          name="city"
          placeholder="City"
          value={user.city}
          onChange={handleChange}
        />
        <br />

        <input
          type="text"
          name="address"
          placeholder="Street / Address"
          value={user.address}
          onChange={handleChange}
        />
        <br />

        <input
          type="text"
          name="phone"
          placeholder="Phone"
          value={user.phone}
          onChange={handleChange}
        />
        <br />

        <button type="submit">Save</button>
      </form>
    </div>
  );
}