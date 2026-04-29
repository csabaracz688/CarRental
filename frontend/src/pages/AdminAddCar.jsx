import { useState } from "react";
import { useNavigate } from "react-router-dom";

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
  const handleChange = (e) => {
    const { name, value } = e.target;

    setCar({
      ...car,
      [name]: value,
    });
  };

  const handleFileChange = (e) => {
    setCar({
      ...car,
      image: e.target.files[0],
    });
  };

  const handleSubmit = (e) => {
    e.preventDefault();

    const formData = new FormData();

    formData.append("LicensePlate", car.licensePlate);
    formData.append("Brand", car.brand);
    formData.append("Model", car.model);
    formData.append("DistanceKm", car.distanceKm);
    formData.append("DailyPrice", car.dailyPrice);
    formData.append("Status", car.status);

    if (car.unavailableFrom)
      formData.append("UnavailableFrom", car.unavailableFrom);

    if (car.unavailableTo)
      formData.append("UnavailableTo", car.unavailableTo);

    if (car.unavailableReason)
      formData.append("UnavailableReason", car.unavailableReason);

    if (car.unavailableNote)
      formData.append("UnavailableNote", car.unavailableNote);

    if (car.image) {
      formData.append("Image", car.image); //  backendnek ezt kell várnia!
    }

    console.log("Küldendő adat:", car);
    console.log(car.image);
    fetch("https://localhost:7077/api/cars", {
      method: "POST",
      headers: {
        Authorization: `Bearer ${localStorage.getItem("token")}`,
      },
      body: formData,
    });
  };

  return (
    <div>

      <button className="back-btn" onClick={() => navigate("/admin")}>
        ← Back
      </button>

      <h1>Add car</h1>

      <form onSubmit={handleSubmit}>
        <input
          name="licensePlate"
          placeholder="License plate"
          onChange={handleChange}
        />
        <br />

        <input name="brand" placeholder="Brand" onChange={handleChange} />
        <br />

        <input name="model" placeholder="Model" onChange={handleChange} />
        <br />

        <input
          type="number"
          name="distanceKm"
          placeholder="Distance (km)"
          onChange={handleChange}
        />
        <br />

        <input
          type="number"
          name="dailyPrice"
          placeholder="Daily price  (USD)"
          onChange={handleChange}
        />
        <br />

        <select name="status" onChange={handleChange}>
          <option value={0}>Available</option>
          <option value={1}>Rented</option>
          <option value={2}>Service</option>
        </select>
        <br />

        <input
          type="datetime-local"
          name="unavailableFrom"
          onChange={handleChange}
        />
        <br />

        <input
          type="datetime-local"
          name="unavailableTo"
          onChange={handleChange}
        />
        <br />

        <input
          type="number"
          name="unavailableReason"
          placeholder="Unavailable reason (enum)"
          onChange={handleChange}
        />
        <br />

        <textarea
          name="unavailableNote"
          placeholder="Megjegyzés"
          onChange={handleChange}
        />
        <br />

        <input type="file" onChange={handleFileChange} />
        <br />

        <button type="submit">Mentés</button>
      </form>
    </div>
  );
}

export default AdminAddCar;