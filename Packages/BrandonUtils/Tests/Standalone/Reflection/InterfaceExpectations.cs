﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using BrandonUtils.Standalone.Collections;

namespace BrandonUtils.Tests.Standalone.Reflection {
    internal static class InterfaceExpectations {
        public static IEnumerable<Type> VehicleTypes  => new List<Type>() { typeof(IVehicle) };
        public static IEnumerable<Type> LandTypes     => new List<Type>() { typeof(ILand) };
        public static IEnumerable<Type> MarineTypes   => new List<Type>() { typeof(IMarine) };
        public static IEnumerable<Type> CarTypes      => new List<Type>() { typeof(ICar), typeof(IVehicle), typeof(ILand) };
        public static IEnumerable<Type> BoatTypes     => new List<Type>() { typeof(IBoat), typeof(IVehicle), typeof(IMarine) };
        public static IEnumerable<Type> DuckTypes     => CarTypes.Union(BoatTypes).Union(typeof(Duckmobile), typeof(object));
        public static IEnumerable<Type> PlaneTypes    => new List<Type>() { typeof(Plane), typeof(IVehicle), typeof(object) };
        public static IEnumerable<Type> WaveTypes     => new List<Type>() { typeof(Wave), typeof(IMarine), typeof(object) };
        public static IEnumerable<Type> TrainTypes    => new List<Type>() { typeof(Train), typeof(ILand), typeof(IVehicle), typeof(object) };
        public static IEnumerable<Type> TrainCarTypes => TrainTypes.Union(typeof(ICar), typeof(TrainCar), typeof(object));
        public static IEnumerable<Type> ConvoyTypes => new List<Type>() {
            typeof(List<ICar>).GetGenericTypeDefinition(),
            typeof(IList<ICar>).GetGenericTypeDefinition(),
            typeof(ICollection<ICar>).GetGenericTypeDefinition(),
            typeof(IEnumerable<ICar>).GetGenericTypeDefinition(),
            typeof(IEnumerable),
            typeof(IReadOnlyCollection<ICar>).GetGenericTypeDefinition(),
            typeof(IReadOnlyList<ICar>).GetGenericTypeDefinition(),
            typeof(IList),
            typeof(ICollection),
            typeof(ILand),
            typeof(Convoy)
        };

        public static readonly IDictionary<Type, IEnumerable<Type>> ExpectedTypes = new Dictionary<Type, IEnumerable<Type>>() {
            [typeof(IVehicle)]   = VehicleTypes,
            [typeof(ILand)]      = LandTypes,
            [typeof(IMarine)]    = MarineTypes,
            [typeof(ICar)]       = CarTypes,
            [typeof(IBoat)]      = BoatTypes,
            [typeof(Duckmobile)] = DuckTypes,
            [typeof(Plane)]      = PlaneTypes,
            [typeof(Wave)]       = WaveTypes,
            [typeof(Train)]      = TrainTypes,
            [typeof(TrainCar)]   = TrainCarTypes,
            [typeof(Convoy)]     = ConvoyTypes
        };
    }

    internal interface IVehicle { }

    internal interface ILand { }

    internal interface IMarine { }

    internal interface ICar : IVehicle, ILand { }

    internal interface IBoat : IVehicle, IMarine { }

    internal class Duckmobile : ICar, IBoat { }

    internal class Plane : IVehicle { }

    internal class Wave : IMarine { }

    internal class Train : IVehicle, ILand { }

    internal class TrainCar : Train, ICar { }

    internal class Convoy : List<IVehicle>, ILand { }
}