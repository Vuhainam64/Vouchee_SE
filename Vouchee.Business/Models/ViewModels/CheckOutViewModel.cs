﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Business.Models.ViewModels
{
    public class CheckOutViewModel
    {
        public Item? item_brief { get; set; }
        public int use_VPoint { get; set; }
        public int use_balance { get; set; }
        public string? gift_email { get; set; }
    }

    public class Item
    {
        public Item()
        {
            modalId = [];
        }

        public IList<Guid> modalId { get; set; }
        public Guid? voucherId { get; set; }
    }
}