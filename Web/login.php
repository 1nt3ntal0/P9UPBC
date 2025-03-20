<?php
header('Content-Type: application/json');

$data = json_decode(file_get_contents('php://input'), true);

$email = $data['email'];
$password = $data['password'];

// Aquí deberías validar los datos y hacer la conexión a la API
$url = 'http://192.168.1.84:5000/login'; // Cambia esto por la URL de tu API
$ch = curl_init($url);

$postData = json_encode([
    'email' => $email,
    'password' => $password
]);

curl_setopt($ch, CURLOPT_POST, 1);
curl_setopt($ch, CURLOPT_POSTFIELDS, $postData);
curl_setopt($ch, CURLOPT_HTTPHEADER, array('Content-Type: application/json'));
curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);

$response = curl_exec($ch);
curl_close($ch);

echo $response;
?>