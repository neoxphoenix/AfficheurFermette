using Projet_AFFICHEURFERMETTE.MDF.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModifieurFermette.Models
{
    public class ExtendedPlat : C_Plat
    {
        private bool _IsUsed;

        public ExtendedPlat(int ID_, string nom_, int Type_, string Photo_, bool IsUsed_) : base(ID_, nom_, Type_, Photo_)
        {
            IsUsed = IsUsed_;
        }
        public ExtendedPlat(C_Plat Plat_, bool IsUsed_) : base(Plat_.ID, Plat_.nom, Plat_.Type, Plat_.Photo)
        {
            IsUsed = IsUsed_;
        }

        public bool IsUsed { get => _IsUsed; set => _IsUsed = value; }
    }
}
