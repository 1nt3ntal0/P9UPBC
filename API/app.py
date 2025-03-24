from flask import Flask, request, jsonify
from flask_sqlalchemy import SQLAlchemy
from werkzeug.security import generate_password_hash, check_password_hash
from flask_cors import CORS
import jwt
import datetime
import os

app = Flask(__name__)
CORS(app)

# Configuración de la base de datos
DB_USER = os.getenv('DB_USER', 'root')
DB_PASSWORD = os.getenv('DB_PASSWORD', '')
DB_HOST = os.getenv('DB_HOST', 'localhost')
DB_NAME = os.getenv('DB_NAME', 'p9upbc')

app.config['SQLALCHEMY_DATABASE_URI'] = f'mysql+pymysql://{DB_USER}:{DB_PASSWORD}@{DB_HOST}/{DB_NAME}'
app.config['SECRET_KEY'] = 'mysecretkey'
db = SQLAlchemy(app)

# Modelo de la tabla 'Empleado'
class Empleado(db.Model):
    __tablename__ = 'empleados'
    id = db.Column(db.Integer, primary_key=True, autoincrement=True)
    username = db.Column(db.String(20), unique=True, nullable=False)
    correo = db.Column(db.String(20), unique=True, nullable=False)
    password = db.Column(db.String(120), nullable=False)

# Modelo de la tabla 'DatosGenerales'
class DatosGenerales(db.Model):
    __tablename__ = 'datos_generales'
    id = db.Column(db.Integer, primary_key=True)
    nombre = db.Column(db.String(100), nullable=False)
    fecha_de_nacimiento = db.Column(db.String(10), nullable=False)
    telefono = db.Column(db.String(10), nullable=False)
    correo = db.Column(db.String(100), nullable=False)
    sangre = db.Column(db.Integer(2), nullable=False)
    religion = db.Column(db.String(100), nullable=False)
    grado = db.Column(db.Integer(2), nullable=False)
    comunicacion = db.Column(db.Integer(2), nullable=False)
    comentarios = db.Column(db.String(200))

# Modelo de la tabla 'Coordenadas'
class Coordenada(db.Model):
    __tablename__ = 'coordenadas'
    id = db.Column(db.Integer, primary_key=True)
    latitude = db.Column(db.Float(6), nullable=False)
    longitude = db.Column(db.Float(6), nullable=False)
    paciente_id = db.Column(db.Integer, db.ForeignKey('datos_generales.id'), nullable=False)

# Endpoint para login
@app.route('/login', methods=['POST'])
def login():
    data = request.json
    username = data.get('username')
    password = data.get('password')

    if not username or not password:
        return jsonify({'message': 'Se requieren username y password'}), 400

    empleado = Empleado.query.filter_by(username=username).first()
    if not empleado or not check_password_hash(empleado.password, password):
        return jsonify({'message': 'Credenciales inválidas'}), 401

    token = jwt.encode({
        'id': empleado.id,
        'exp': datetime.datetime.utcnow() + datetime.timedelta(hours=1)
    }, app.config['SECRET_KEY'], algorithm='HS256')

    token_str = token.decode('utf-8') if isinstance(token, bytes) else token

    return jsonify({
        'Id': empleado.id,
        'Token': token_str
    })

# Endpoint para obtener la última coordenada de un paciente
@app.route('/coordenadas/<int:paciente_id>', methods=['GET'])
def obtener_coordenadas(paciente_id):
    ultima_coordenada = Coordenada.query.filter_by(paciente_id=paciente_id).order_by(Coordenada.id.desc()).first()
    
    if not ultima_coordenada:
        return jsonify({'message': 'No se encontraron coordenadas para el paciente'}), 404

    return jsonify({
        'id': ultima_coordenada.id,
        'latitude': ultima_coordenada.latitude,
        'longitude': ultima_coordenada.longitude,
        'paciente_id': ultima_coordenada.paciente_id
    })

# Endpoint para obtener coordenadas por fecha
@app.route('/coordenadas/por-fecha', methods=['GET'])
def obtener_coordenadas_por_fecha():
    paciente_id = request.args.get('paciente_id')
    fecha = request.args.get('fecha')

    if not paciente_id or not fecha:
        return jsonify({'message': 'Se requieren los parámetros "paciente_id" y "fecha"'}), 400

    try:
        paciente_id = int(paciente_id)
    except ValueError:
        return jsonify({'message': 'El parámetro "paciente_id" debe ser un número entero'}), 400

    try:
        datetime.strptime(fecha, '%d-%m-%Y')
    except ValueError:
        return jsonify({'message': 'Formato de fecha inválido. Use el formato DD-MM-AAAA'}), 400

    coordenadas = Coordenada.query.filter(
        Coordenada.paciente_id == paciente_id,
        Coordenada.fecha == fecha
    ).order_by(
        db.func.STR_TO_DATE(Coordenada.fecha, '%d-%m-%Y').asc()
    ).all()

    if not coordenadas:
        return jsonify({'message': 'No se encontraron coordenadas para el paciente y la fecha especificados'}), 404

    coordenadas_formateadas = [
        {'latitude': coord.latitude, 'longitude': coord.longitude}
        for coord in coordenadas
    ]

    return jsonify(coordenadas_formateadas)

# Endpoint para actualizar datos generales
@app.route('/datos-generales/<int:id>', methods=['PUT'])
def actualizar_datos_generales(id):
    datos = DatosGenerales.query.get(id)
    if not datos:
        return jsonify({'message': 'Datos generales no encontrados'}), 404

    data = request.json
    if 'nombre' in data:
        datos.nombre = data['nombre']
    if 'fecha_de_nacimiento' in data:
        datos.fecha_de_nacimiento = data['fecha_de_nacimiento']
    if 'telefono' in data:
        datos.telefono = data['telefono']
    if 'correo' in data:
        datos.correo = data['correo']
    if 'sangre' in data:
        datos.sangre = data['sangre']
    if 'religion' in data:
        datos.religion = data['religion']
    if 'grado' in data:
        datos.grado = data['grado']
    if 'comunicacion' in data:
        datos.comunicacion = data['comunicacion']
    if 'comentarios' in data:
        datos.comentarios = data['comentarios']

    db.session.commit()
    return jsonify({'message': 'Datos generales actualizados exitosamente'})

# Ejecutar la aplicación
if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5000, debug=True)