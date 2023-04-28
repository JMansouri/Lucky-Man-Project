
using UnityEngine;

namespace LuckyMan.Runtime
{
    public class DiceSimulator
    {
        public static int GetRandomDiceNumber()
        {
            int rand = Random.Range(1,6);
            return rand;
        }
    }
}
