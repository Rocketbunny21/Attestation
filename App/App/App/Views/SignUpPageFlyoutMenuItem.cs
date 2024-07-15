using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Views
{
    public class SignUpPageFlyoutMenuItem
    {
        public SignUpPageFlyoutMenuItem()
        {
            TargetType = typeof(SignUpPageFlyoutMenuItem);
        }
        public int Id { get; set; }
        public string Title { get; set; }

        public Type TargetType { get; set; }
    }
}