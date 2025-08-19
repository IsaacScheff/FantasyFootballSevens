using UnityEngine;

public static class Dice {
    public static int D8() { return Random.Range(1, 9); }
    public static int D6() { return Random.Range(1, 7); }
    public static int Roll2D6() { return D6() + D6(); }
}
