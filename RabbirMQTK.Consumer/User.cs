using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQTK.Consumer
{

    public class User
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }


        public override string ToString()
        {
            return $@"  {Id}
                        {Name}
                        {Email}
                        {Password} ";
        }
    }
}
