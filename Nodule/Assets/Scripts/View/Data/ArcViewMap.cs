using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Core.Data;
using Assets.Scripts.View.Items;

namespace Assets.Scripts.View.Data
{
    public class ArcViewMap
    {
        private readonly IDictionary<PointDir, ArcView> _arcMap = new Dictionary<PointDir, ArcView>();
        private readonly IDictionary<Point, HashSet<ArcView>> _arcSet = new Dictionary<Point, HashSet<ArcView>>();

        public IEnumerable<ArcView> Arcs
        {
            get { return _arcMap.Values; }
        }

        public void Reset(ArcViewMap arcViewMap)
        {
            _arcMap.Clear();
            _arcSet.Clear();

            foreach (var pair in arcViewMap._arcMap) {
                _arcMap.Add(pair.Key, pair.Value);
            }

            foreach (var pair in arcViewMap._arcSet) {
                _arcSet.Add(pair.Key, pair.Value);
            }
        }

        public bool ContainsArc(Point pos, Direction dir)
        {
            return _arcMap.ContainsKey(new PointDir(pos, dir));
        }

        public ICollection<ArcView> GetArcs(Point pos)
        {
            HashSet<ArcView> arcViews;
            if (_arcSet.TryGetValue(pos, out arcViews)) {
                return arcViews;
            }

            arcViews = new HashSet<ArcView>();
            _arcSet.Add(pos, arcViews);
            return arcViews;
        }

        public bool TryGetArc(Point pos, Direction dir, out ArcView arcView)
        {
            return _arcMap.TryGetValue(new PointDir(pos, dir), out arcView);
        }

        public void Add(Point pos, Direction dir, ArcView arcView)
        {
            _arcMap.Add(new PointDir(pos, dir), arcView);

            HashSet<ArcView> arcs;
            if (_arcSet.TryGetValue(pos, out arcs)) {
                arcs.Add(arcView);
                return;
            }

            arcs = new HashSet<ArcView> {arcView};
            _arcSet.Add(pos, arcs);
        }

        public bool Remove(Point pos)
        {
            Directions.All
                .Select(dir => new PointDir(pos, dir))
                .ToList()
                .ForEach(pointDir => _arcMap.Remove(pointDir)); // Remove all possible point directions
            return _arcSet.Remove(pos);
        }

        public void Clear()
        {
            _arcMap.Clear();
            _arcSet.Clear();
        }
    }
}
