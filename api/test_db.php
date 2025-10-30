<?php
$message = '';

if ($_SERVER['REQUEST_METHOD'] === 'POST') {
    $host = trim($_POST['host']);
    $dbname = trim($_POST['dbname']);
    $user = trim($_POST['user']);
    $pass = trim($_POST['pass']);

    try {
        $pdo = new PDO(
            "mysql:host=$host;dbname=$dbname;charset=utf8mb4",
            $user,
            $pass,
            [PDO::ATTR_ERRMODE => PDO::ERRMODE_EXCEPTION]
        );
        $message = "<span style='color:green;'>✅ Connection successful!</span>";
    } catch (PDOException $e) {
        $message = "<span style='color:red;'>❌ Connection failed: " . htmlspecialchars($e->getMessage()) . "</span>";
    }
}
?>

<!DOCTYPE html>
<html lang="en">
<head>
<meta charset="UTF-8">
<title>Test DB Connection</title>
<style>
body { font-family: Arial, sans-serif; padding: 20px; }
input { padding: 5px; width: 300px; margin-bottom: 10px; }
label { display: block; margin-top: 10px; }
button { padding: 7px 15px; }
.message { margin-top: 15px; }
</style>
</head>
<body>
<h2>Test MySQL Database Connection</h2>

<form method="post">
    <label>Host:
        <input type="text" name="host" value="<?= htmlspecialchars($_POST['host'] ?? 'localhost') ?>" required>
    </label>
    <label>Database Name:
        <input type="text" name="dbname" value="<?= htmlspecialchars($_POST['dbname'] ?? '') ?>" required>
    </label>
    <label>Username:
        <input type="text" name="user" value="<?= htmlspecialchars($_POST['user'] ?? '') ?>" required>
    </label>
    <label>Password:
        <input type="password" name="pass" value="<?= htmlspecialchars($_POST['pass'] ?? '') ?>">
    </label>
    <button type="submit">Test Connection</button>
</form>

<div class="message"><?= $message ?></div>
</body>
</html>
