using System;
using System.Collections.Generic;
using System.Text;

namespace EntityLayer.GaleriaArte
{
    public class Usuarios
    {
        public Usuarios() { }

        public Usuarios(int id, string nickname, string correo, string contraseñaHash, string rol, bool estado, DateTime fechaCreacion)
        {
            this.id = id;
            this.nickname = nickname;
            this.correo = correo;
            this.contraseñaHash = contraseñaHash;
            this.rol = rol;
            this.estado = estado;
            this.fechaCreacion = fechaCreacion;
        }
        
        public int id{ get; set; }
        public string nickname { get; set; }
        public string correo { get; set; }
        public string contraseñaHash { get; set; }
        public string rol { get; set; }
        public bool estado { get; set; }
        public DateTime fechaCreacion{get; set;}

    }
}
