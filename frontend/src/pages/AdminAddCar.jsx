import { useState } from "react";
import { useNavigate } from "react-router-dom";
import "../styles/AdminAddCar.css";
import { useToast } from "../components/Toaster";

function AdminAddCar() {
  const [car, setCar] = useState({
    licensePlate: "",
    brand: "",
    model: "",
    distanceKm: "",
    dailyPrice: "",
    status: 0,
    unavailableFrom: "",
    unavailableTo: "",
    unavailableReason: "",
    unavailableNote: "",
    image: null,
  });

  const navigate = useNavigate();
  const toast = useToast();
  const [saving, setSaving] = useState(false);
  const [previewUrl, setPreviewUrl] = useState(null);
  const handleChange = (e) => {
    const { name, value } = e.target;

    setCar({
      ...car,
      [name]: value,
    });
  };

  const handleFileChange = (e) => {
    const file = e.target.files[0];
    setCar({
      ...car,
      image: file,
    });
    if (file) setPreviewUrl(URL.createObjectURL(file));
  };
  const handleSubmit = async (e) => {
    e.preventDefault();

    // basic client-side validation
    if (!car.licensePlate || !car.brand || !car.model || !car.dailyPrice) {
      toast("Please fill required fields (license, brand, model, price).", { type: "warning" });
      return;
    }

    setSaving(true);

    try {
      const formData = new FormData();

      formData.append("LicensePlate", car.licensePlate);
      formData.append("Brand", car.brand);
      formData.append("Model", car.model);
      formData.append("DistanceKm", car.distanceKm);
      formData.append("DailyPrice", car.dailyPrice);
      formData.append("Status", car.status);

      if (car.unavailableFrom) formData.append("UnavailableFrom", car.unavailableFrom);
      if (car.unavailableTo) formData.append("UnavailableTo", car.unavailableTo);
      if (car.unavailableReason) formData.append("UnavailableReason", car.unavailableReason);
      if (car.unavailableNote) formData.append("UnavailableNote", car.unavailableNote);
      if (car.image) formData.append("Image", car.image);

      const res = await fetch("https://localhost:7077/api/cars", {
        method: "POST",
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
        body: formData,
      });

      if (!res.ok) {
        const err = await res.text().catch(() => null);
        console.error("Create car failed:", res.status, err);
        toast(err || "Unable to create car.", { type: "error" });
        return;
      }

      toast("Car created successfully.", { type: "success" });
      navigate("/admin");
    } catch (err) {
      console.error("Create car error:", err);
      toast("Unable to create car.", { type: "error" });
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className="admin-add-car-page page-shell">
      <button className="back-btn" onClick={() => navigate("/admin")}>
        ← Back
      </button>

      <div className="page-head">
        <div>
          <h1 className="page-title">Add Car</h1>
          <p className="page-subtitle">Create a new vehicle in the fleet inventory.</p>
        </div>
      </div>

      <form className="admin-add-car-form card-surface" onSubmit={handleSubmit}>
        <input
          name="licensePlate"
          placeholder="License plate"
          onChange={handleChange}
        />

        <input name="brand" placeholder="Brand" onChange={handleChange} />

        <input name="model" placeholder="Model" onChange={handleChange} />

        <input
          type="number"
          name="distanceKm"
          placeholder="Distance (km)"
          onChange={handleChange}
        />

        <input
          type="number"
          name="dailyPrice"
          placeholder="Daily price (Ft)"
          onChange={handleChange}
        />

        <select name="status" onChange={handleChange}>
          <option value={0}>Available</option>
          <option value={1}>Rented</option>
          <option value={2}>Service</option>
        </select>

        <input
          type="datetime-local"
          name="unavailableFrom"
          onChange={handleChange}
        />

        <input
          type="datetime-local"
          name="unavailableTo"
          onChange={handleChange}
        />

        <input
          type="number"
          name="unavailableReason"
          placeholder="Unavailable reason (enum)"
          onChange={handleChange}
        />

        <textarea
          name="unavailableNote"
          placeholder="Note"
          onChange={handleChange}
        />

        <input type="file" onChange={handleFileChange} />

        <button className="btn-primary" type="submit">Save</button>
      </form>
    </div>
  );
}

export default AdminAddCar;