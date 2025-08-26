using UnityEngine;

public static class Dice {
    public static int D8() { return Random.Range(1, 9); }
    public static int D6() { return Random.Range(1, 7); }
    public static int Roll2D6(out int d1, out int d2) { d1 = D6(); d2 = D6(); return d1 + d2; }
}