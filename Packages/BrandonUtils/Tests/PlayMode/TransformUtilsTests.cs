using System.Collections.Generic;

using BrandonUtils.Logging;
using BrandonUtils.Spatial;
using BrandonUtils.Standalone.Collections;

using NUnit.Framework;

using UnityEngine;

namespace BrandonUtils.Tests.PlayMode {
    public class TransformUtilsTests {
        [Test]
        public void localVerpTest() {
            //TODO: Finish this test after Ludum Dare 48
            var obj1_world = new Vector3(2, 2);
            var obj1_rot   = 45;
            var obj2_world = new Vector3(-4,   2);
            var ojb2_local = new Vector3(-3,   3);
            var verp       = new Vector3(0.5f, 0, 0);
            var expected   = new Vector3(0.5f, 0.5f);

            Debug.Log("creating a gameobject");
            var obj1 = new GameObject("obj1");
            Debug.Log("done");
            obj1.transform.position    = obj1_world;
            obj1.transform.eulerAngles = new Vector3(0, 0, obj1_rot);

            var tPoint = obj1.transform.TransformPoint(obj2_world);
            var tVec   = obj1.transform.TransformVector(obj2_world);
            var tDir   = obj1.transform.TransformDirection(obj2_world);

            var msg = $"{nameof(obj1)}:\n";
            msg += new Dictionary<object, object>() {
                {"pos", obj1.transform.position},
                {"rot", obj1.transform.eulerAngles},
                {"left", -obj1.transform.right}
            }.JoinString("\n");
            LogUtils.Log(msg);

            LogUtils.Log($"{nameof(tPoint)} = {tPoint}", $"{nameof(tVec)} = {tVec}", $"{nameof(tDir)} = {tDir}");

            var actual = TransformUtils.LocalVerp(obj1.transform, obj2_world, verp);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void tloc2() {
            var ob1 = new GameObject("ob1").transform;
            ob1.position    = new Vector3(2, 2);
            ob1.eulerAngles = new Vector3(0, 0, 45);
            var ob2_local = new Vector3(3, 4); // h = 5
            var ob2_world = (ob1.position) + (ob1.right * ob2_local.x) + (ob1.up * ob2_local.y);

            LogUtils.Log(
                new Dictionary<object, object>() {
                    {$"{nameof(ob2_local)} -> {nameof(ob2_world)}", $"{ob2_local} -> {ob2_world}"},
                    {nameof(ob1.TransformPoint), ob1.TransformPoint(ob2_local)},
                    {nameof(ob1.TransformVector), ob1.TransformVector(ob2_local)},
                    {nameof(ob1.TransformDirection), ob1.TransformDirection(ob2_local)},
                    {"", ""},
                    {$"{nameof(ob2_world)} -> {nameof(ob2_local)}", $"{ob2_world} -> {ob2_local}"},
                    {nameof(ob1.InverseTransformPoint), ob1.InverseTransformPoint(ob2_world)},
                    {nameof(ob1.InverseTransformVector), ob1.InverseTransformVector(ob2_world)},
                    {nameof(ob1.InverseTransformDirection), ob1.InverseTransformDirection(ob2_world)},
                }
            );
        }
    }
}
