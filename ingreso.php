<?php
session_start();

require("conexion.php");


$tipo_doc = $_POST["tipo_doc"];
$documento = $_POST["documento"];
$usuario = $_POST["usuario"];
$password = $_POST["password"];


$sql = "SELECT *
        FROM usuarios
        WHERE tipo_doc = ?
        AND documento = ?
        AND usuario = ?
        AND password = ?";

$stmt = $conexion->prepare($sql);


$stmt->bind_param("ssss", $tipo_doc, $documento, $usuario, $password);

$stmt->execute();

$resultado = $stmt->get_result();

if($resultado->num_rows == 1)
{
    $fila = $resultado->fetch_assoc();

    $_SESSION["documento"] = $fila["documento"];
    $_SESSION["usuario"] = $fila["usuario"];

    header("Location: resumen.php");
}
else
{
    echo "<h2>Usuario o contraseña incorrectos.</h2>";
}

$stmt->close();
$conexion->close();
?>