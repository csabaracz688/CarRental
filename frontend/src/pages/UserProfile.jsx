import { useEffect, useState } from "react";

export default function Profile() {
  const [user, setUser] = useState({
    address: "",
    phone: "",
  });

  const userId = localStorage.getItem("userId");

  useEffect(() => {
    fetch(`http://localhost:5168/api/users/${userId}`)
      .then((res) => res.json())
      .then((data) => {
        setUser({
          address: data.address || "",
          phone: data.phone || "",
        });
      });
  }, [userId]);

  const handleChange = (e) => {
    setUser({ ...user, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    await fetch(`http://localhost:5168/api/users/${userId}`, {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(user),
    });

    alert("Profile updated!");
  };

  return (
    <div>
  <h1>Profile</h1>

  <form onSubmit={handleSubmit}>
    
    <input
      type="text"
      name="postalCode"
      placeholder="Postal Code"
      value={user.postalCode || ""}
      onChange={handleChange}
    />
    <br />

    <input
      type="text"
      name="city"
      placeholder="City"
      value={user.city || ""}
      onChange={handleChange}
    />
    <br />

    <input
      type="text"
      name="addressLine"
      placeholder="Street / Address"
      value={user.addressLine || ""}
      onChange={handleChange}
    />
    <br />

    <input
      type="text"
      name="phone"
      placeholder="Phone"
      value={user.phone || ""}
      onChange={handleChange}
    />
    <br />

    <button type="submit">Save</button>
  </form>
</div>
  );
}