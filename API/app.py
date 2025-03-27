from flask import Flask, request, jsonify
from flask_sqlalchemy import SQLAlchemy
from werkzeug.security import generate_password_hash, check_password_hash
import jwt
import datetime
import os

app = Flask(__name__)

# Configuración de la base de datos
DB_USER = os.getenv('DB_USER', 'root')
DB_PASSWORD = os.getenv('DB_PASSWORD', '')
DB_HOST = os.getenv('DB_HOST', 'localhost')
DB_NAME = os.getenv('DB_NAME', 'p9upbc')

app.config['SQLALCHEMY_DATABASE_URI'] = f'mysql+pymysql://{DB_USER}:{DB_PASSWORD}@{DB_HOST}/{DB_NAME}'
app.config['SECRET_KEY'] = 'mysecretkey'
db = SQLAlchemy(app)


# Modelo de la tabla 'usuarios'
class Usuario(db.Model):
    __tablename__ = 'usuarios'
    Id = db.Column(db.Integer, primary_key=True)
    Usuario = db.Column(db.String(80), unique=True, nullable=False)
    Password = db.Column(db.String(120), nullable=False)
    pacientes = db.relationship('Paciente', backref='owner', lazy=True)

# Modelo de la tabla 'paciente'
class Paciente(db.Model):
    __tablename__ = 'paciente'
    FkID = db.Column(db.Integer, db.ForeignKey('usuarios.Id'), primary_key=True, nullable=False)
    nombre = db.Column(db.String(100), primary_key=True, nullable=False)
    edad = db.Column(db.Integer, nullable=False)
    sangre = db.Column(db.String(10))
    religion = db.Column(db.String(50))
    Grado = db.Column(db.String(50))
    Extra = db.Column(db.Text)
    Telefono = db.Column(db.String(15))
    coordenadas = db.relationship('Coordenada', backref='paciente', lazy=True)

# Modelo de la tabla 'coordenadas'
class Coordenada(db.Model):
    __tablename__ = 'coordenadas'
    id = db.Column(db.Integer, primary_key=True)
    latitude = db.Column(db.String(50), nullable=False)
    longitude = db.Column(db.String(50), nullable=False)
    paciente_id = db.Column(db.Integer, db.ForeignKey('paciente.FkID'), nullable=False)

# Crear todas las tablas en la base de datos
with app.app_context():
    db.create_all()

@app.route('/register', methods=['POST'])  # CREATE: Registrar un nuevo usuario
def register():
    data = request.json
    if not data.get('username') or not data.get('password'):
        return jsonify({'message': 'Se requieren el nombre de usuario y la contraseña'}), 400

    # Verificar si el usuario ya existe
    if Usuario.query.filter_by(Usuario=data['username']).first():
        return jsonify({'message': 'El nombre de usuario ya está en uso'}), 400

    hashed_password = generate_password_hash(data['password'], method='pbkdf2:sha256')
    new_user = Usuario(Usuario=data['username'], Password=hashed_password)
    db.session.add(new_user)
    db.session.commit()
    return jsonify({'message': 'Usuario creado exitosamente', 'id': new_user.Id}), 201

@app.route('/login', methods=['POST'])  # LOGIN: Iniciar sesión y obtener token
def login():
    data = request.json
    if not data.get('username') or not data.get('password'):
        return jsonify({'message': 'Se requieren el nombre de usuario y la contraseña'}), 400

    user = Usuario.query.filter_by(Usuario=data['username']).first()
    if not user or not check_password_hash(user.Password, data['password']):
        return jsonify({'message': 'Credenciales inválidas'}), 401

    token = jwt.encode(
        {'sub': user.Usuario, 'exp': datetime.datetime.utcnow() + datetime.timedelta(hours=1)},
        app.config['SECRET_KEY'],
        algorithm='HS256'
    )
    return jsonify({'access_token': token})


@app.route('/add_paciente', methods=['POST'])  # CREATE: Agregar un paciente
def add_paciente():
    data = request.json
    if not data.get('FkID') or not data.get('nombre') or not data.get('edad'):
        return jsonify({'message': 'Se requieren FkID, nombre y edad'}), 400

    # Verificar si el FkID existe en la tabla de usuarios
    usuario_existente = Usuario.query.get(data['FkID'])
    if not usuario_existente:
        return jsonify({'message': 'El FkID no existe en la tabla de usuarios'}), 404

    new_paciente = Paciente(
        FkID=data['FkID'],
        nombre=data['nombre'],
        edad=data['edad'],
        sangre=data.get('sangre'),
        religion=data.get('religion'),
        Grado=data.get('Grado'),
        Extra=data.get('Extra'),
        Telefono=data.get('Telefono')
    )
    db.session.add(new_paciente)
    db.session.commit()
    return jsonify({'message': 'Paciente agregado exitosamente'}), 201

@app.route('/update_paciente', methods=['PUT'])  # UPDATE: Modificar un paciente
def update_paciente():
    data = request.json
    if not data.get('FkID') or not data.get('nombre'):
        return jsonify({'message': 'Se requieren el ID del paciente y el nombre'}), 400

    paciente = Paciente.query.filter_by(FkID=data['FkID'], nombre=data['nombre']).first()
    if not paciente:
        return jsonify({'message': 'Paciente no encontrado'}), 404

    # Actualizar campos permitidos
    if 'edad' in data:
        paciente.edad = data['edad']
    if 'religion' in data:
        paciente.religion = data['religion']
    if 'Grado' in data:
        paciente.Grado = data['Grado']
    if 'Extra' in data:
        paciente.Extra = data['Extra']
    if 'Telefono' in data:
        paciente.Telefono = data['Telefono']

    db.session.commit()
    return jsonify({'message': 'Paciente actualizado exitosamente'})

@app.route('/delete_paciente', methods=['DELETE'])  # DELETE: Eliminar un paciente
def delete_paciente():
    data = request.json
    if not data.get('FkID') or not data.get('nombre'):
        return jsonify({'message': 'Se requieren el ID del paciente y el nombre'}), 400

    paciente = Paciente.query.filter_by(FkID=data['FkID'], nombre=data['nombre']).first()
    if not paciente:
        return jsonify({'message': 'Paciente no encontrado'}), 404

    db.session.delete(paciente)
    db.session.commit()
    return jsonify({'message': 'Paciente eliminado exitosamente'})

@app.route('/add_coordenadas', methods=['POST'])  # CREATE: Agregar coordenadas
def add_coordenadas():
    data = request.json
    if not data.get('latitude') or not data.get('longitude') or not data.get('paciente_id'):
        return jsonify({'message': 'Se requieren latitude, longitude y paciente_id'}), 400

    # Verificar si el paciente existe
    paciente_existente = Paciente.query.get(data['paciente_id'])
    if not paciente_existente:
        return jsonify({'message': 'El paciente_id no existe en la tabla de pacientes'}), 404

    new_coord = Coordenada(
        latitude=data['latitude'],
        longitude=data['longitude'],
        paciente_id=data['paciente_id']
    )
    db.session.add(new_coord)
    db.session.commit()
    return jsonify({'message': 'Coordenadas agregadas exitosamente'}), 201

@app.route('/get_coordenadas', methods=['GET'])  # GET: Obtener coordenadas de un paciente
def get_coordenadas():
    paciente_id = request.args.get('paciente_id')
    if not paciente_id:
        return jsonify({'message': 'Se requiere el ID del paciente'}), 400

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

if __name__ == '__main__':
    app.run(debug=True)