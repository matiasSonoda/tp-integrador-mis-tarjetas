<?php
require_once("conexion.php");

    if ($_SERVER["REQUEST_METHOD"] != "POST") {
        exit();
    }
    // Guardo los datos del formulario en variables
    $tipo_doc = $_POST["tipo_doc"];
    $documento = $_POST["documento"];
    $nombre = $_POST["nombre"];
    $apellido = $_POST["apellido"];
    $fecha_nacimiento = $_POST["fecha_nacimiento"];
    $email = $_POST["email"];
    $usuario = $_POST["usuario"];
    $passwordA = $_POST["passwordA"];
    $passwordB = $_POST["passwordB"];
    

    if ($passwordA !== $passwordB) {
        
        echo "Las contraseñas no coinciden.";
        exit();
    }
    $password = $passwordA;

    if ($tipo_doc !== "DNI" && $tipo_doc !== "PASAPORTE") {
        echo "Tipo de documento no válido.";
        exit();        
    }

    if (empty($documento) ||
        empty($nombre) ||
        empty($apellido) ||
        empty($email) ||
        empty($usuario) ||
        empty($passwordA) ||
        empty($passwordB)) {
    echo "Debe completar todos los campos.";
    exit();
    }

    $sql = "SELECT * FROM usuarios WHERE documento = '$documento'";
    $resultado = $conexion->query($sql);

    if ($resultado->num_rows === 0) {
        echo "No existe un usuario con ese documento.";
        exit();
    }

    $fila = $resultado->fetch_assoc();
    if ($fila["usuario"] !== NULL && $fila["password"] !== NULL) {
    echo "La cuenta ya fue activada.";
    exit();
    }


    $sql = "SELECT * FROM tarjetas WHERE dni_titular = '$documento'";
    $resultado = $conexion->query($sql);

    if ($resultado->num_rows === 0) {
        echo "No existe una tarjeta asociada a ese documento.";
        exit();
    }


    $sql = "UPDATE usuarios
            SET usuario = '$usuario', password = '$password'
            WHERE documento = '$documento'";

    if ($conexion->query($sql) === TRUE) {
        echo "Su cuenta se activó correctamente.";
    } else {
        echo "Error al activar la cuenta: " . $conexion->error;
    }

    $conexion->close();
?>