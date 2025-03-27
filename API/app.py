from flask import Flask, request, jsonify
from flask_sqlalchemy import SQLAlchemy
from werkzeug.security import generate_password_hash, check_password_hash
from flask_cors import CORS  # Importar CORS
import jwt
import datetime
import os

app = Flask(__name__)

# Habilitar CORS para todas las rutas
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

# Modelo de la tabla 'Clientes'
class Cliente(db.Model):
    __tablename__ = 'clientes'
    id = db.Column(db.Integer, primary_key=True)
    nombre = db.Column(db.String(100), nullable=False)
    password = db.Column(db.String(120), nullable=False)

# Modelo de la tabla 'DatosGenerales'
class DatosGenerales(db.Model):
    __tablename__ = 'datos_generales'
    id = db.Column(db.Integer, primary_key=True)
    nombre = db.Column(db.String(100), nullable=False)
    fecha_de_nacimiento = db.Column(db.String(10), nullable=False)
    telefono = db.Column(db.String(10), nullable=False)
    correo = db.Column(db.String(100), nullable=False)
    sangre = db.Column(db.Integer, nullable=False)  # Entero para tipo de sangre
    religion = db.Column(db.String(100), nullable=False)
    grado = db.Column(db.Integer, nullable=False)  # Entero para grado
    comunicacion = db.Column(db.Integer, nullable=False)  # Entero para comunicación
    comentarios = db.Column(db.String(200))

# Modelo de la tabla 'Coordenadas'
class Coordenada(db.Model):
    __tablename__ = 'coordenadas'
    id = db.Column(db.Integer, primary_key=True)
    latitude = db.Column(db.Float(6), nullable=False)
    longitude = db.Column(db.Float(6), nullable=False)
    paciente_id = db.Column(db.Integer, db.ForeignKey('datos_generales.id'), nullable=False)

# Crear todas las tablas en la base de datos
with app.app_context():
    db.create_all()

# Métodos para la tabla 'Empleado'
@app.route('/empleados', methods=['POST'])
def crear_empleado():
    data = request.json
    if not data.get('username') or not data.get('correo') or not data.get('password'):
        return jsonify({'message': 'Se requieren username, correo y password'}), 400

    # Verificar si el empleado ya existe
    if Empleado.query.filter_by(username=data['username']).first():
        return jsonify({'message': 'El username ya está en uso'}), 400

    hashed_password = generate_password_hash(data['password'], method='pbkdf2:sha256')
    nuevo_empleado = Empleado(
        username=data['username'],
        correo=data['correo'],
        password=hashed_password
    )
    db.session.add(nuevo_empleado)
    db.session.commit()
    return jsonify({'message': 'Empleado creado exitosamente', 'id': nuevo_empleado.id}), 201

@app.route('/empleados/<int:id>', methods=['DELETE'])
def eliminar_empleado(id):
    empleado = Empleado.query.get(id)
    if not empleado:
        return jsonify({'message': 'Empleado no encontrado'}), 404

    db.session.delete(empleado)
    db.session.commit()
    return jsonify({'message': 'Empleado eliminado exitosamente'})

# Métodos para la tabla 'Clientes'
@app.route('/clientes', methods=['POST'])
def crear_cliente():
    data = request.json
    if not data.get('nombre') or not data.get('password'):
        return jsonify({'message': 'Se requieren nombre y password'}), 400

    hashed_password = generate_password_hash(data['password'], method='pbkdf2:sha256')
    nuevo_cliente = Cliente(
        nombre=data['nombre'],
        password=hashed_password
    )
    db.session.add(nuevo_cliente)
    db.session.commit()
    return jsonify({'message': 'Cliente creado exitosamente', 'id': nuevo_cliente.id}), 201

@app.route('/clientes/<int:id>', methods=['GET'])
def obtener_cliente(id):
    cliente = Cliente.query.get(id)
    if not cliente:
        return jsonify({'message': 'Cliente no encontrado'}), 404

    return jsonify({
        'id': cliente.id,
        'nombre': cliente.nombre
    })

@app.route('/clientes/<int:id>', methods=['PUT'])
def actualizar_cliente(id):
    cliente = Cliente.query.get(id)
    if not cliente:
        return jsonify({'message': 'Cliente no encontrado'}), 404

    data = request.json
    if 'nombre' in data:
        cliente.nombre = data['nombre']
    if 'password' in data:
        cliente.password = generate_password_hash(data['password'], method='pbkdf2:sha256')

    db.session.commit()
    return jsonify({'message': 'Cliente actualizado exitosamente'})

@app.route('/clientes/<int:id>', methods=['DELETE'])
def eliminar_cliente(id):
    cliente = Cliente.query.get(id)
    if not cliente:
        return jsonify({'message': 'Cliente no encontrado'}), 404

    db.session.delete(cliente)
    db.session.commit()
    return jsonify({'message': 'Cliente eliminado exitosamente'})

# Métodos para la tabla 'DatosGenerales'
@app.route('/datos-generales', methods=['POST'])
def crear_datos_generales():
    data = request.json
    if not data.get('nombre') or not data.get('fecha_de_nacimiento') or not data.get('telefono'):
        return jsonify({'message': 'Se requieren nombre, fecha_de_nacimiento y telefono'}), 400

    nuevos_datos = DatosGenerales(
        nombre=data['nombre'],
        fecha_de_nacimiento=data['fecha_de_nacimiento'],
        telefono=data['telefono'],
        correo=data.get('correo'),
        sangre=data.get('sangre'),  # Entero
        religion=data.get('religion'),
        grado=data.get('grado'),  # Entero
        comunicacion=data.get('comunicacion'),  # Entero
        comentarios=data.get('comentarios')
    )
    db.session.add(nuevos_datos)
    db.session.commit()
    return jsonify({'message': 'Datos generales creados exitosamente', 'id': nuevos_datos.id}), 201

@app.route('/datos-generales/<int:id>', methods=['GET'])
def obtener_datos_generales(id):
    datos = DatosGenerales.query.get(id)
    if not datos:
        return jsonify({'message': 'Datos generales no encontrados'}), 404

    return jsonify({
        'id': datos.id,
        'nombre': datos.nombre,
        'fecha_de_nacimiento': datos.fecha_de_nacimiento,
        'telefono': datos.telefono,
        'correo': datos.correo,
        'sangre': datos.sangre,  # Entero
        'religion': datos.religion,
        'grado': datos.grado,  # Entero
        'comunicacion': datos.comunicacion,  # Entero
        'comentarios': datos.comentarios
    })

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
        datos.sangre = data['sangre']  # Entero
    if 'religion' in data:
        datos.religion = data['religion']
    if 'grado' in data:
        datos.grado = data['grado']  # Entero
    if 'comunicacion' in data:
        datos.comunicacion = data['comunicacion']  # Entero
    if 'comentarios' in data:
        datos.comentarios = data['comentarios']

    db.session.commit()
    return jsonify({'message': 'Datos generales actualizados exitosamente'})

@app.route('/datos-generales/<int:id>', methods=['DELETE'])
def eliminar_datos_generales(id):
    datos = DatosGenerales.query.get(id)
    if not datos:
        return jsonify({'message': 'Datos generales no encontrados'}), 404

    db.session.delete(datos)
    db.session.commit()
    return jsonify({'message': 'Datos generales eliminados exitosamente'})

# Métodos para la tabla 'Coordenadas'
@app.route('/coordenadas', methods=['POST'])
def crear_coordenadas():
    data = request.json
    if not data.get('latitude') or not data.get('longitude') or not data.get('paciente_id'):
        return jsonify({'message': 'Se requieren latitude, longitude y paciente_id'}), 400

    nueva_coordenada = Coordenada(
        latitude=data['latitude'],
        longitude=data['longitude'],
        paciente_id=data['paciente_id']
    )
    db.session.add(nueva_coordenada)
    db.session.commit()
    return jsonify({'message': 'Coordenadas creadas exitosamente', 'id': nueva_coordenada.id}), 201

@app.route('/coordenadas/<int:paciente_id>', methods=['GET'])
def obtener_coordenadas(paciente_id):
    coordenadas = Coordenada.query.filter_by(paciente_id=paciente_id).all()
    if not coordenadas:
        return jsonify({'message': 'No se encontraron coordenadas para el paciente'}), 404

    coordenadas_json = [
        {
            'id': coord.id,
            'latitude': coord.latitude,
            'longitude': coord.longitude,
            'paciente_id': coord.paciente_id
        }
        for coord in coordenadas
    ]
    return jsonify(coordenadas_json)

# Ejecutar la aplicación
if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5000, debug=True)  # Escuchar en todas las interfaces