﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Xml.Linq;
using Dominio;

namespace Negocio
{
    public class ArticleList
    {
        public List<Article> Show()
        {
            List<Article> list = new List<Article>();
            AccesoDatos data = new AccesoDatos();


            try
            {   
                //ejecuto opción de query para traer los datos que necesito de las tablas correspondientes con un join
                data.setQuery("Select A.Id, Codigo, Nombre, A.Descripcion, ImagenUrl, Precio, M.Descripcion as Marca, C.Descripcion as Categoria,IdMarca ,idCategoria  \r\nfrom ARTICULOS A, MARCAS M, CATEGORIAS C\r\nWhere A.IdMarca = M.Id and A.IdCategoria = C.Id");
                data.ejecutarLectura();

                while (data.Lector.Read())
                {
                    //Se cargan los articulos de la base y verifico Null en todos los campos (dado que la tabla acepta NULLS)
                    Article aux = new Article();
                    aux.Id = (int)data.Lector["Id"];
                    if (!(data.Lector["Codigo"] is DBNull))
                        aux.code = (string)data.Lector["Codigo"];
                    if (!(data.Lector["Nombre"] is DBNull))
                        aux.name = (string)data.Lector["Nombre"];
                    if (!(data.Lector["Descripcion"] is DBNull))
                        aux.description = (string)data.Lector["Descripcion"];
                    aux.brand = new Marca();
                    aux.brand.Id = (int)data.Lector["IdMarca"];
                    if (!(data.Lector["Marca"] is DBNull))
                        aux.brand.Descripcion = (string)data.Lector["Marca"];
                    aux.category = new Categoria();
                    aux.category.Id = (int)data.Lector["idCategoria"];
                    if (!(data.Lector["Categoria"] is DBNull))
                        aux.category.Descripcion = (string)data.Lector["Categoria"];
                    if (!(data.Lector["ImagenUrl"] is DBNull))
                        aux.img = (string)data.Lector["ImagenUrl"];
                    if(!(data.Lector["Precio"] is DBNull))
                        aux.price = (decimal)data.Lector["Precio"];

                    //Se agrega el registro leído a la lista de articulos
                    list.Add(aux); 

                }

                //devuelvo listado de articulos
                return list;
            }

            catch(Exception ex)
            {
                throw ex;
            }

            finally
            {   //se cierra la conección a DB
                data.cerrarConexion();
            }
        }

        public void Add(Article newArticle)
        { 
             //Se abre la conección a DB
            AccesoDatos datos = new AccesoDatos();

            try
            {   //Se incerta en DB los datos cargados en la plantilla "agregar", pueden ser null
                datos.setQuery("Insert into ARTICULOS (Codigo, Nombre, Descripcion, IdMarca, IdCategoria, ImagenUrl, Precio)values('" + newArticle.code + "','" + newArticle.name + "','" + newArticle.description + "', @idBrand, @idCategory, @ImagenUrl,'" + newArticle.price + "')");
                datos.setearParametro("@idBrand", newArticle.brand.Id);
                datos.setearParametro("@idCategory", newArticle.category.Id);
                datos.setearParametro("@ImagenUrl", newArticle.img);
                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {   //Se abre la conección a DB
                datos.cerrarConexion();
            }
        }

        public void Modify(Article modArticle)
        {
            //Se abre la conección a DB
            AccesoDatos datos = new AccesoDatos();

            try
            {   //Se incerta en DB los datos cargados en la plantilla "modificar"
                datos.setQuery("UPDATE ARTICULOS set Codigo=@code, Nombre=@name, Descripcion=@description, IdMarca=@idBrand, IdCategoria=@idCategory, ImagenUrl=@ImagenUrl, Precio=@Price where Id=@id");
                datos.setearParametro("@id", modArticle.Id);
                datos.setearParametro("@idBrand", modArticle.brand.Id);
                datos.setearParametro("@idCategory", modArticle.category.Id);
                datos.setearParametro("@ImagenUrl", modArticle.img);
                datos.setearParametro("@code", modArticle.code);
                datos.setearParametro("@name", modArticle.name);
                datos.setearParametro("@description", modArticle.description);
                datos.setearParametro("@Price", modArticle.price);
                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {   //Se cierra la conección a DB
                datos.cerrarConexion();
            }
        }

        public void Drop(int id)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {   //Se inserta en DB los datos cargados en la plantilla "modificar"
                datos.setQuery("delete from ARTICULOS where id=@id");
                datos.setearParametro("@id", id);
                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {   //Se abre la conexión a DB
                datos.cerrarConexion();
            }
        }

        public List<Article> filtrar(string buscarPor, string criterio, string filtro)
        {
            List<Article> lista = new List<Article>();
            AccesoDatos datos = new AccesoDatos();

            try
            {
                string consulta = "Select A.Id, Codigo, Nombre, A.Descripcion, Precio, M.Descripcion as Marca, C.Descripcion as Categoria from ARTICULOS A, MARCAS M, CATEGORIAS C Where A.IdMarca = M.Id and A.IdCategoria = C.Id and ";
                if (buscarPor == "Precio")
                {
                    switch (criterio)
                    {
                        case "Mayor a":
                            consulta += "A.Precio > " + filtro;
                            break;
                        case "Menor a":
                            consulta += "A.Precio < " + filtro;
                            break;
                        case "Igual a":
                            consulta += "A.Precio = " + filtro;
                            break;
                    }
                }
                else
                {
                    string campo;
                    switch (buscarPor)
                    {
                        case "Código":
                            campo = "A.Codigo";
                            break;
                        case "Nombre artículo":
                            campo = "A.Nombre";
                            break;
                        default:
                            campo = "A.Descripcion";
                            break;
                    }
                    switch (criterio)
                    {
                        case "Igual a":
                            consulta += campo + "like '" + filtro + "'";
                            break;
                        case "Contiene":
                            consulta += campo + "like '%" + filtro + "%'";
                            break;
                        case "Comienza con":
                            consulta += campo + "like '%" + filtro + "'";
                            break;
                        case "Termina con":
                            consulta += campo + "like '" + filtro + "%'";
                            break;
                    }
                }

                datos.setQuery(consulta);
                datos.ejecutarLectura();
                while (datos.Lector.Read())
                {
                    //Se cargan los articulos de la base y verifico Null en todos los campos (dado que la tabla acepta NULLS)
                    Article aux = new Article();
                    aux.Id = (int)datos.Lector["Id"];
                    if (!(datos.Lector["Codigo"] is DBNull))
                        aux.code = (string)datos.Lector["Codigo"];
                    if (!(datos.Lector["Nombre"] is DBNull))
                        aux.name = (string)datos.Lector["Nombre"];
                    if (!(datos.Lector["Descripcion"] is DBNull))
                        aux.description = (string)datos.Lector["Descripcion"];
                    aux.brand = new Marca();
                    aux.brand.Id = (int)datos.Lector["IdMarca"];
                    if (!(datos.Lector["Marca"] is DBNull))
                        aux.brand.Descripcion = (string)datos.Lector["Marca"];
                    aux.category = new Categoria();
                    aux.category.Id = (int)datos.Lector["idCategoria"];
                    if (!(datos.Lector["Categoria"] is DBNull))
                        aux.category.Descripcion = (string)datos.Lector["Categoria"];
                    if (!(datos.Lector["ImagenUrl"] is DBNull))
                        aux.img = (string)datos.Lector["ImagenUrl"];
                    if (!(datos.Lector["Precio"] is DBNull))
                        aux.price = (decimal)datos.Lector["Precio"];

                    //Se agrega el registro leído a la lista de articulos
                    lista.Add(aux);

                }
                return lista;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
