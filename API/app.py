# Importaciones
from flask import Flask, request, jsonify
from flask_sqlalchemy import SQLAlchemy
from werkzeug.security import generate_password_hash, check_password_hash
import jwt
import datetime
import os

# Configuración inicial de la aplicación
app = Flask(__name__)

# Configuración de la base de datos
DB_USER = os.getenv('DB_USER', 'root')
DB_PASSWORD = os.getenv('DB_PASSWORD', '')
DB_HOST = os.getenv('DB_HOST', 'localhost')
DB_NAME = os.getenv('DB_NAME', 'p9upbc')

app.config['SQLALCHEMY_DATABASE_URI'] = f'mysql+pymysql://{DB_USER}:{DB_PASSWORD}@{DB_HOST}/{DB_NAME}'
app.config['SECRET_KEY'] = 'mysecretkey'
db = SQLAlchemy(app)

# Modelos de la base de datos
class User(db.Model):
    __tablename__ = 'users'
    Id = db.Column(db.Integer, primary_key=True)
    Username = db.Column(db.String(80), unique=True, nullable=False)
    Password = db.Column(db.String(120), nullable=False)
    patients = db.relationship('Patient', backref='owner', lazy=True)

class Patient(db.Model):
    __tablename__ = 'patient'
    FkID = db.Column(db.Integer, db.ForeignKey('users.Id'), primary_key=True, nullable=False)
    name = db.Column(db.String(100), primary_key=True, nullable=False)
    age = db.Column(db.Integer, nullable=False)
    blood_type = db.Column(db.Integer)
    religion = db.Column(db.String(50))
    grade = db.Column(db.Integer)
    extra = db.Column(db.Integer)
    phone = db.Column(db.String(15))

class Coordinate(db.Model):
    __tablename__ = 'coordinates'
    id = db.Column(db.Integer, primary_key=True)
    latitude = db.Column(db.String(50), nullable=False)
    longitude = db.Column(db.String(50), nullable=False)
    patient_id = db.Column(db.Integer, db.ForeignKey('patient.FkID'), nullable=False)

# Crear tablas en la base de datos
with app.app_context():
    db.create_all()

# Endpoints de autenticación y usuarios
@app.route('/register', methods=['POST'])
def register():
    data = request.json
    if not data.get('username') or not data.get('password'):
        return jsonify({'message': 'Username and password are required'}), 400
    if User.query.filter_by(Username=data['username']).first():
        return jsonify({'message': 'Username already exists'}), 400
    
    hashed_password = generate_password_hash(data['password'], method='pbkdf2:sha256')
    new_user = User(Username=data['username'], Password=hashed_password)
    db.session.add(new_user)
    db.session.commit()
    return jsonify({'message': 'User created successfully', 'id': new_user.Id}), 201

@app.route('/login', methods=['POST'])
def login():
    data = request.json
    if not data.get('username') or not data.get('password'):
        return jsonify({'message': 'Username and password are required'}), 400
    
    user = User.query.filter_by(Username=data['username']).first()
    if not user or not check_password_hash(user.Password, data['password']):
        return jsonify({'message': 'Invalid credentials'}), 401
    
    token = jwt.encode(
        {'sub': user.Username, 'exp': datetime.datetime.utcnow() + datetime.timedelta(hours=1)},
        app.config['SECRET_KEY'],
        algorithm='HS256'
    )
    return jsonify({'id': user.Id, 'access_token': token})

@app.route('/users/<int:user_id>', methods=['GET'])
def get_user(user_id):
    user = User.query.get(user_id)
    if not user:
        return jsonify({'message': 'User not found'}), 404
    return jsonify({
        'Id': user.Id,
        'Username': user.Username
    })

@app.route('/update_user', methods=['PUT'])
def update_user():
    data = request.json
    user = User.query.get(data.get('Id'))
    if not user:
        return jsonify({'message': 'User not found'}), 404
    
    if 'Username' in data:
        user.Username = data['Username']
    if 'Password' in data:
        user.Password = generate_password_hash(data['Password'])
    
    db.session.commit()
    return jsonify({'message': 'User updated successfully'})

# Endpoints de pacientes
@app.route('/patients/<int:user_id>', methods=['GET'])
def get_patient(user_id):
    patient = Patient.query.filter_by(FkID=user_id).first()
    if not patient:
        return jsonify({'message': 'Patient not found'}), 404
    
    return jsonify({
        'FkID': patient.FkID,
        'name': patient.name,
        'age': patient.age,
        'blood_type': patient.blood_type,
        'religion': patient.religion,
        'grade': patient.grade,
        'extra': patient.extra,
        'phone': patient.phone
    })

@app.route('/add_patient', methods=['POST'])
def add_patient():
    data = request.json
    if not data.get('FkID') or not data.get('name') or not data.get('age'):
        return jsonify({'message': 'FkID, name and age are required'}), 400
    
    existing_user = User.query.get(data['FkID'])
    if not existing_user:
        return jsonify({'message': 'User not found'}), 404
    
    new_patient = Patient(
        FkID=data['FkID'],
        name=data['name'],
        age=data['age'],
        blood_type=data.get('blood_type'),
        religion=data.get('religion'),
        grade=data.get('grade'),
        extra=data.get('extra'),
        phone=data.get('phone')
    )
    db.session.add(new_patient)
    db.session.commit()
    return jsonify({'message': 'Patient added successfully'}), 201

@app.route('/update_patient', methods=['PUT'])
def update_patient():
    data = request.json
    if not data.get('FkID') or not data.get('name'):
        return jsonify({'message': 'FkID and name are required'}), 400
    
    patient = Patient.query.filter_by(FkID=data['FkID'], name=data['name']).first()
    if not patient:
        return jsonify({'message': 'Patient not found'}), 404
    
    if 'age' in data: patient.age = data['age']
    if 'blood_type' in data: patient.blood_type = data['blood_type']
    if 'religion' in data: patient.religion = data['religion']
    if 'grade' in data: patient.grade = data['grade']
    if 'extra' in data: patient.extra = data['extra']
    if 'phone' in data: patient.phone = data['phone']
    
    db.session.commit()
    return jsonify({'message': 'Patient updated successfully'})

@app.route('/delete_patient', methods=['DELETE'])
def delete_patient():
    data = request.json
    if not data.get('FkID') or not data.get('name'):
        return jsonify({'message': 'FkID and name are required'}), 400
    
    patient = Patient.query.filter_by(FkID=data['FkID'], name=data['name']).first()
    if not patient:
        return jsonify({'message': 'Patient not found'}), 404
    
    db.session.delete(patient)
    db.session.commit()
    return jsonify({'message': 'Patient deleted successfully'})

# Endpoints de coordenadas
@app.route('/latest_coordinates/<int:user_id>', methods=['GET'])
def get_latest_coordinates(user_id):
    try:
        coordinate = Coordinate.query.filter_by(patient_id=user_id)\
                                   .order_by(Coordinate.id.desc())\
                                   .first()
        
        if not coordinate:
            return jsonify({
                'latitude': 0.0,
                'longitude': 0.0
            }), 200
        
        lat = float(coordinate.latitude)
        lng = float(coordinate.longitude)
        
        return jsonify({
            'latitude': lat,
            'longitude': lng
        })
        
    except Exception as e:
        print(f"Error al obtener coordenadas: {str(e)}")
        return jsonify({
            'latitude': 0.0,
            'longitude': 0.0
        }), 200

@app.route('/add_coordinates', methods=['POST'])
def add_coordinates():
    data = request.json
    if not data.get('latitude') or not data.get('longitude') or not data.get('patient_id'):
        return jsonify({'message': 'Latitude, longitude and patient_id are required'}), 400
    
    patient = Patient.query.get(data['patient_id'])
    if not patient:
        return jsonify({'message': 'Patient not found'}), 404
    
    new_coordinate = Coordinate(
        latitude=data['latitude'],
        longitude=data['longitude'],
        patient_id=data['patient_id']
    )
    db.session.add(new_coordinate)
    db.session.commit()
    return jsonify({'message': 'Coordinates added successfully'}), 201

@app.route('/coordinates/<int:patient_id>', methods=['GET'])
def get_coordinates(patient_id):
    coordinates = Coordinate.query.filter_by(patient_id=patient_id).all()
    if not coordinates:
        return jsonify({'message': 'No coordinates found'}), 404
    
    return jsonify([{
        'id': c.id,
        'latitude': float(c.latitude),
        'longitude': float(c.longitude),
        'timestamp': c.timestamp.strftime("%Y-%m-%d %H:%M:%S")
    } for c in coordinates])

@app.route('/delete_coordinate/<int:coord_id>', methods=['DELETE'])
def delete_coordinate(coord_id):
    coordinate = Coordinate.query.get(coord_id)
    if not coordinate:
        return jsonify({'message': 'Coordinate not found'}), 404
    
    db.session.delete(coordinate)
    db.session.commit()
    return jsonify({'message': 'Coordinate deleted successfully'})

# Punto de entrada de la aplicación
if __name__ == '__main__':
    app.run(debug=True)