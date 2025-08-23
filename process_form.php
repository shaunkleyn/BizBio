<?php
// These details are for connecting to your MySQL database
// The username, password, and database name are all found in cPanel
$servername = "localhost:3306"; // Usually 'localhost'
$username = "shaunkle_shaunkleyn"; // The one you use for cPanel
$password = "koe47qEwU4y6mJSZ3EtRw8M63jBfbfaB"; // The password for your database user
$dbname = "shaunkle_bizbio"; // The name of the database you created

// Create a connection
$conn = new mysqli($servername, $username, $password, $dbname);

// Check the connection for errors
if ($conn->connect_error) {
  die("Connection failed: " . $conn->connect_error);
}

// Get the data from your HTML form
$name = $_POST['name'];

// SQL query to insert the data
$sql = "INSERT INTO users (name) VALUES ('$name')";

// Execute the query
if ($conn->query($sql) === TRUE) {
  echo "Data was successfully saved to the database.";
} else {
  echo "Error: " . $sql . "<br>" . $conn->error;
}

// Close the connection
$conn->close();
?>