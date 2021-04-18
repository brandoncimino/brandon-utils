using System;
using System.Collections.Generic;

namespace BrandonUtils.Standalone.Collections {
    /// <summary>
    /// Represents an pair of arbitrary, strongly-typed values.
    /// <p/>
    /// TODO: Create an annotation to remove the label. (Maybe this works with existing attributes?)
    /// TODO: Create an annotation to set custom labels to either of the 2 pairs.
    /// TODO: Create an annotation to make the "primary" value read-only in the editor.
    /// TODO: Make a special kind of collection for Pairs, OR a custom renderer for Lists of pairs
    /// </summary>
    /// <remarks>
    /// This is essentially a <see cref="KeyValuePair{TKey,TValue}"/> version of <see cref="Vector2"/>. (We cannot extend <see cref="UnityEngine"/> directly because it is <see langword="sealed"/>)
    /// </remarks>
    /// <example>
    /// This can be used to specify things like ranges of values as single variables:
    /// <code><![CDATA[
    /// public int minPlayers;
    /// public int maxPlayers;
    /// ]]></code>
    ///
    /// You could use a <see cref="Pair{TX,TY}"/>, but that would allow floats, and this should be restricted to ints:
    /// <code><![CDATA[
    /// public Vector2 playerRange;
    /// ]]></code>
    ///
    /// By using a <see cref="Pair{T1,T2}"/>, you can restrict the values to ints:
    /// <code><![CDATA[
    /// public Pair<int,int> playerRange;
    /// ]]></code>
    ///
    /// This also lets you take advantage of other <see cref="SerializableAttribute"/> fields in the Unity editor:
    /// <code><![CDATA[
    /// public Pair<DayOfWeek,Color> MondayColor;
    /// ]]></code>
    /// </example>
    /// <typeparam name="TX"></typeparam>
    /// <typeparam name="TY"></typeparam>
    [Serializable]
    public struct Pair<TX, TY> {
        public TX X;
        public TY Y;

        public Pair(TX x = default, TY y = default) {
            X = x;
            Y = y;
        }

        public override string ToString() {
            return $"[{X},{Y}]";
        }
    }
}