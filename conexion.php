<?php
    $servidor = "localhost";
    $usuario = "root";
    $password = "";
    $baseDatos = "mi_banco_db";

    $conexion = new mysqli($servidor, $usuario, $password, $baseDatos);

    if ($conexion->connect_error) {
        die("Conexión fallida: " . $conexion->connect_error);
    } else {
        echo "Conexión exitosa a MySQL <br>";
    }
    
    $conexion->close();
?>