<?php
use Psr\Http\Message\ResponseInterface as Response;
use Psr\Http\Message\ServerRequestInterface as Request;

return function($app) {

    // Home route
    $app->get('/', function(Request $request, Response $response) {
        $response->getBody()->write(json_encode(["message" => "API is working!"]));
        return $response->withHeader('Content-Type', 'application/json');
    });

    // Get all users
    $app->get('/users', function(Request $request, Response $response) {
        $db = $request->getAttribute('db');
        $stmt = $db->query("SELECT id, name, email FROM users");
        $users = $stmt->fetchAll();
        $response->getBody()->write(json_encode($users));
        return $response->withHeader('Content-Type', 'application/json');
    });

    // Create a new user
    $app->post('/users', function(Request $request, Response $response) {
        $db = $request->getAttribute('db');
        $data = $request->getParsedBody();
        $stmt = $db->prepare("INSERT INTO users (name, email) VALUES (?, ?)");
        $stmt->execute([$data['name'], $data['email']]);
        $response->getBody()->write(json_encode(["status" => "success"]));
        return $response->withHeader('Content-Type', 'application/json');
    });

};
