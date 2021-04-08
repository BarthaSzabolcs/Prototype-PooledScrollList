using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.MarketPlace
{
    [Serializable]
    public class GenericTest<T1, T2>
    {
        [SerializeField] private T1 template1;
        [SerializeField] private T2 template2;
    }
}
