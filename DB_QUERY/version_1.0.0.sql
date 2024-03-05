USE master;
DROP DATABASE IF EXISTS portafolio;
CREATE DATABASE portafolio;

USE portafolio;

/* TIPO ENTRADA */
CREATE TABLE format(
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
);

/* CATEGORIAS */
CREATE TABLE category (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(100) NOT NULL
);

/* USUARIO */
CREATE TABLE users (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    rol VARCHAR(100) NOT NULL,
    img VARCHAR(100) NOT NULL,
    cv VARCHAR(100) NOT NULL,
    about TEXT NOT NULL,
    hobbies TEXT NOT NULL,
    email VARCHAR(100) NOT NULL,
    password VARCHAR(100) NOT NULL,
);

/* ENTRADAS */
CREATE TABLE post (
    id INT IDENTITY(1,1) PRIMARY KEY,
    title VARCHAR(100) NOT NULL,
    description TEXT NOT NULL,
    cover VARCHAR(100) NOT NULL,
    category_id INT NOT NULL,
    FOREIGN KEY (category_id) REFERENCES category(id),
    user_id INT NOT NULL,
    FOREIGN KEY (user_id) REFERENCES users(id),
    format_id INT NOT NULL,
    FOREIGN KEY (format_id) REFERENCES format(id),
    created_at DATE NOT NULL DEFAULT GETDATE()
);

/* FUENTE */
CREATE TABLE source(
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    icon VARCHAR(100) NOT NULL
);

/* LINKS */
CREATE TABLE link(
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    url VARCHAR(100) NOT NULL,
    post_id INT NOT NULL,
    FOREIGN KEY (post_id) REFERENCES post(id),
    source_id INT NOT NULL,
    FOREIGN KEY (source_id) REFERENCES source(id)
);

/* MEDIA TYPE */
CREATE TABLE mediatype(
    id INT IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
);

/* MEDIA */
CREATE TABLE media (
    id INT IDENTITY(1,1) PRIMARY KEY,
    url VARCHAR(100) NOT NULL,
    post_id INT NOT NULL,
    FOREIGN KEY (post_id) REFERENCES post(id),
    mediatype_id INT NOT NULL,
    FOREIGN KEY (mediatype_id) REFERENCES mediatype(id)
);


/*  SOCIAL NETWORKS */
CREATE TABLE social_network (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    url VARCHAR(100) NOT NULL,
    username VARCHAR(100) NOT NULL,
    icon VARCHAR(100) NOT NULL,
    user_id INT NOT NULL,
    FOREIGN KEY (user_id) REFERENCES users(id)
);

/* BIO */
CREATE TABLE bio(
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    work VARCHAR(100) NOT NULL,
    year INT NOT NULL,
    user_id INT NOT NULL,
    FOREIGN KEY (user_id) REFERENCES users(id)
);


/* CREACIONES DE PRUEBA */

-- CREATE USER
INSERT INTO users (name, rol, img, cv, about, hobbies, email, password) 
VALUES ('Jhon Doe', 'Developer', 'https://via.placeholder.com/150', 'https://via.placeholder.com/150', 'Lorem ipsum dolor sit amet consectetur adipisicing elit. Quos, quae.', 'Lorem ipsum dolor sit amet consectetur adipisicing elit. Quos, quae.',
'Jhon@mail.com', '123456');

-- CREATE FORMATS
INSERT INTO format (name) VALUES ('Blog'), ('Project'), ('Tutorial');

--CREAR CATEGORIAS
INSERT INTO category (name) VALUES ('Web Development'), ('C++'), ('C#'), ('.NET');

--INSER POST
INSERT INTO post (title, description, cover, category_id, user_id, format_id, created_at)
VALUES ('Post 1', 'Lorem ipsum dolor sit amet consectetur adipisicing elit. Quos, quae.', 'https://via.placeholder.com/150', 2, 1, 1, GETDATE());

--SOURCE
INSERT INTO source (name, icon)
VALUES ('Github', 'fa-brands fa-github'),('vmeo', 'fa-brands fa-vmeo'), ('You Tube', 'fa-brands fa-youtube'),('Instargram', 'fa-brands fa-instagram'),('URL', 'fa-solid fa-link');

--INSERT MEDIA TYPE
INSERT INTO mediatype (name)
VALUES ('img'), ('video'), ('file');

/* SELECTS */
-- SELECT * FROM users;
-- SELECT * FROM format;
-- SELECT * FROM category;
SELECT * FROM post;
-- SELECT * FROM source
SELECT * FROM mediatype
SELECT * FROM media
/* SELECTS INNER JOIN */
-- SELECT P.id, P.title, P.description, P.cover, P.created_at, U.name as userName, F.name as formatName, C.name as categoryName FROM post P
-- INNER JOIN category C ON P.category_id = C.id
-- INNER JOIN users U ON P.user_id = U.id
-- INNER JOIN format F ON P.format_id = F.id;