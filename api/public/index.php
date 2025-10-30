<?php
require __DIR__ . '/../vendor/autoload.php';

use Slim\Factory\AppFactory;

// Load config
$config = require __DIR__ . '/../config.php';

// Create App
$app = AppFactory::create();

// Create DB connection
$dsn = "mysql:host={$config['db']['host']};dbname={$config['db']['dbname']};charset={$config['db']['charset']}";
$db = new PDO($dsn, $config['db']['user'], $config['db']['pass'], [
    PDO::ATTR_ERRMODE => PDO::ERRMODE_EXCEPTION,
    PDO::ATTR_DEFAULT_FETCH_MODE => PDO::FETCH_ASSOC
]);

// Share $db with routes via middleware
$app->add(function ($request, $handler) use ($db) {
    $request = $request->withAttribute('db', $db);
    return $handler->handle($request);
});

// Load routes
(require __DIR__ . '/../src/routes.php')($app);

$app->run();
