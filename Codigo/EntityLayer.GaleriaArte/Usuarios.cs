using System;
using System.Collections.Generic;
using System.Text;

namespace EntityLayer.GaleriaArte
{
    public class Usuarios
    {
        public Usuarios() { }

        public Usuarios(int id, string nickname, string correo, string contraseña_hash, string rol, bool estado, DateTime fecha_creacion)
        {
            this.id = id;
            this.nickname = nickname;
            this.correo = correo;
            this.contraseña_hash = contraseña_hash;
            this.rol = rol;
            this.estado = estado;
            this.fecha_creacion = fecha_creacion;
        }
        
        public int id{ get; set; }
        public string nickname { get; set; }
        public string correo { get; set; }
        public string contraseña_hash { get; set; }
        public string rol { get; set; }
        public bool estado { get; set; }
        public DateTime fecha_creacion { get; set;}

    }
}
