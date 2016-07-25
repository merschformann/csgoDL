using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace csgoDL
{
    public class AppViewModel
    {
        public ICollectionView WeaponProgressionT { get; private set; }

        public List<WeaponProgressionElement> WeaponProgressionTSource { get; internal set; }

        public ICollectionView WeaponProgressionCT { get; private set; }

        public List<WeaponProgressionElement> WeaponProgressionCTSource { get; internal set; }

        public AppViewModel(List<WeaponProgressionElement> weaponProgressionT, List<WeaponProgressionElement> weaponProgressionCT)
        {
            WeaponProgressionTSource = weaponProgressionT;
            WeaponProgressionCTSource = weaponProgressionCT;
            WeaponProgressionT = CollectionViewSource.GetDefaultView(WeaponProgressionTSource);
            WeaponProgressionCT = CollectionViewSource.GetDefaultView(WeaponProgressionCTSource);
        }

        public AppViewModel()
        {
            WeaponProgressionTSource = new List<WeaponProgressionElement>()
            {
                new WeaponProgressionElement(){ Type = WeaponType.glock, Count = 1},
                new WeaponProgressionElement(){ Type = WeaponType.hkp2000, Count = 1},
                new WeaponProgressionElement(){ Type = WeaponType.tec9, Count = 1},
                new WeaponProgressionElement(){ Type = WeaponType.p250, Count = 1},
                new WeaponProgressionElement(){ Type = WeaponType.deagle, Count = 1},
                new WeaponProgressionElement(){ Type = WeaponType.fiveseven, Count = 1},
                new WeaponProgressionElement(){ Type = WeaponType.elite, Count = 1},
                new WeaponProgressionElement(){ Type = WeaponType.mp9, Count = 1},
                new WeaponProgressionElement(){ Type = WeaponType.mac10, Count = 1},
                new WeaponProgressionElement(){ Type = WeaponType.mp7, Count = 1},
                new WeaponProgressionElement(){ Type = WeaponType.bizon, Count = 1},
                new WeaponProgressionElement(){ Type = WeaponType.ump45, Count = 1},
                new WeaponProgressionElement(){ Type = WeaponType.p90, Count = 1},
                new WeaponProgressionElement(){ Type = WeaponType.nova, Count = 1},
                new WeaponProgressionElement(){ Type = WeaponType.mag7, Count = 1},
                new WeaponProgressionElement(){ Type = WeaponType.xm1014, Count = 1},
                new WeaponProgressionElement(){ Type = WeaponType.sawedoff, Count = 1},
                new WeaponProgressionElement(){ Type = WeaponType.galilar, Count = 1},
                new WeaponProgressionElement(){ Type = WeaponType.famas, Count = 1},
                new WeaponProgressionElement(){ Type = WeaponType.ak47, Count = 1},
                new WeaponProgressionElement(){ Type = WeaponType.m4a1, Count = 1},
                new WeaponProgressionElement(){ Type = WeaponType.sg556, Count = 1},
                new WeaponProgressionElement(){ Type = WeaponType.aug, Count = 1},
                new WeaponProgressionElement(){ Type = WeaponType.awp, Count = 1},
                new WeaponProgressionElement(){ Type = WeaponType.m249, Count = 1},
                new WeaponProgressionElement(){ Type = WeaponType.negev, Count = 1},
            };

            WeaponProgressionCTSource = new List<WeaponProgressionElement>()
            {
                new WeaponProgressionElement(){ Type = WeaponType.glock, Count = 1},
                new WeaponProgressionElement(){ Type = WeaponType.hkp2000, Count = 1},
                new WeaponProgressionElement(){ Type = WeaponType.tec9, Count = 1},
                new WeaponProgressionElement(){ Type = WeaponType.p250, Count = 1},
                new WeaponProgressionElement(){ Type = WeaponType.deagle, Count = 1},
                new WeaponProgressionElement(){ Type = WeaponType.fiveseven, Count = 1},
                new WeaponProgressionElement(){ Type = WeaponType.elite, Count = 1},
                new WeaponProgressionElement(){ Type = WeaponType.mp9, Count = 1},
                new WeaponProgressionElement(){ Type = WeaponType.mac10, Count = 1},
                new WeaponProgressionElement(){ Type = WeaponType.mp7, Count = 1},
                new WeaponProgressionElement(){ Type = WeaponType.bizon, Count = 1},
                new WeaponProgressionElement(){ Type = WeaponType.ump45, Count = 1},
                new WeaponProgressionElement(){ Type = WeaponType.p90, Count = 1},
                new WeaponProgressionElement(){ Type = WeaponType.nova, Count = 1},
                new WeaponProgressionElement(){ Type = WeaponType.mag7, Count = 1},
                new WeaponProgressionElement(){ Type = WeaponType.xm1014, Count = 1},
                new WeaponProgressionElement(){ Type = WeaponType.sawedoff, Count = 1},
                new WeaponProgressionElement(){ Type = WeaponType.galilar, Count = 1},
                new WeaponProgressionElement(){ Type = WeaponType.famas, Count = 1},
                new WeaponProgressionElement(){ Type = WeaponType.ak47, Count = 1},
                new WeaponProgressionElement(){ Type = WeaponType.m4a1, Count = 1},
                new WeaponProgressionElement(){ Type = WeaponType.sg556, Count = 1},
                new WeaponProgressionElement(){ Type = WeaponType.aug, Count = 1},
                new WeaponProgressionElement(){ Type = WeaponType.awp, Count = 1},
                new WeaponProgressionElement(){ Type = WeaponType.m249, Count = 1},
                new WeaponProgressionElement(){ Type = WeaponType.negev, Count = 1},
            };

            WeaponProgressionT = CollectionViewSource.GetDefaultView(WeaponProgressionTSource);

            WeaponProgressionCT = CollectionViewSource.GetDefaultView(WeaponProgressionCTSource);
        }
    }
}
