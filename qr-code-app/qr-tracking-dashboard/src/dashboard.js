import React, { useEffect, useState } from "react";
import { Bar } from "react-chartjs-2";
import "chart.js/auto";

const Dashboard = () => {
  const [analytics, setAnalytics] = useState([]); // Ensure it starts as an array
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    fetch("http://localhost:3000/analytics")
      .then((res) => res.json())
      .then((data) => {
        if (Array.isArray(data)) {
          setAnalytics(data);
        } else {
          console.error("Unexpected API response:", data);
          setError("Invalid API response");
        }
        setLoading(false);
      })
      .catch((err) => {
        console.error("Error fetching analytics:", err);
        setError("Failed to load data");
        setLoading(false);
      });
  }, []);

  if (loading) return <p>Loading analytics...</p>;
  if (error) return <p style={{ color: "red" }}>{error}</p>;

  const chartData = {
    labels: analytics.map((item) => `QR ${item.qr_id}`),
    datasets: [
      {
        label: "Total Scans",
        data: analytics.map((item) => item.scans),
        backgroundColor: "#36A2EB",
      },
      {
        label: "Unique Visitors",
        data: analytics.map((item) => item.unique_visitors),
        backgroundColor: "#FF6384",
      },
    ],
  };

  return (
    <div className="container" style={{ maxWidth: "600px", margin: "20px auto" }}>
      <h1>QR Code Analytics</h1>
      <Bar data={chartData} />
    </div>
  );
};

export default Dashboard;
