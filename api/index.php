<?php
require __DIR__ . '/vendor/autoload.php';

use Slim\Factory\AppFactory;

// Load config
$config = require __DIR__ . '/config.php';

// Create Slim App
$app = AppFactory::create();

// Create DB connection
$db = mysqli_connect($config['db']['host'], $config['db']['user'], $config['db']['pass'], $config['db']['dbname']) or die(mysql_error()); // make connection to mysql


// Middleware: inject DB into requests
$app->add(function ($request, $handler) use ($db) {
    $request = $request->withAttribute('db', $db);
    return $handler->handle($request);
});

// Load routes
(require __DIR__ . '/src/routes.php')($app);

// Run app
$app->run();
