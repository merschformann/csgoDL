using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csgoDL
{
    /// <summary>
    /// All available weapon types.
    /// </summary>
    public enum WeaponType
    {
        mp9,
        mac10,
        mp7,
        bizon,
        ump45,
        p90,
        nova,
        mag7,
        xm1014,
        sawedoff,
        galilar,
        famas,
        ak47,
        m4a1,
        m4a1_silencer,
        sg556,
        aug,
        ssg08,
        awp,
        g3sg1,
        scar20,
        m249,
        negev,
        glock,
        hkp2000,
        usp_silencer,
        cz75a,
        tec9,
        p250,
        deagle,
        fiveseven,
        elite,
        knifegg
    }

    public class WeaponProgressionElement : INotifyPropertyChanged
    {
        private WeaponType _type;

        public WeaponType Type { get { return _type; } set { _type = value; NotifyPropertyChanged("Type"); } }

        private int _count;

        public int Count { get { return _count; } set { _count = value; NotifyPropertyChanged("Count"); } }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
