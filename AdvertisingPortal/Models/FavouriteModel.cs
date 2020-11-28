using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AdvertisingPortal.Models {
    [Table("Favourites")]
    public class FavouriteModel {
        public int ID { get; set; }
        public virtual AdvertisementModel Advertisement { get; set; }
        public virtual UserModel User { get; set; }

        // widoki, kontrolery dodawania usuwania, 
    }
}