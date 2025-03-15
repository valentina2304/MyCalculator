using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tema1Calculator
{
    class MemoryManager
    {
        private double _memory;
        private List<double> _memoryList;

        public MemoryManager()
        {
            _memory = 0;
            _memoryList = new List<double>();
        }

        public void MemoryStore(double value)
        {
            _memory = value;
            _memoryList.Add(_memory);
        }

        public List<double> GetMemoryList()
        {
            return new List<double>(_memoryList);
        }

        public void ClearMemoryList()
        {
            _memoryList.Clear();
            _memory = 0;
        }

        public double RecallMemory()
        {
            if (_memoryList.Count == 0)
                return 0;

            int lastIndex = _memoryList.Count - 1;
            double lastValue = _memoryList[lastIndex];
            return lastValue;
        }

        public void MemoryAdd(double currentValue)
        {
            if (_memoryList.Count > 0)
            {
                int lastIndex = _memoryList.Count - 1;
                double lastValue = _memoryList[lastIndex];
                _memoryList[lastIndex] = lastValue + currentValue;
                _memory = _memoryList[lastIndex];
            }
            else
            {
                _memory = currentValue;
                _memoryList.Add(_memory);
            }
        }

        public void MemorySubtract(double currentValue)
        {
            if (_memoryList.Count > 0)
            {
                int lastIndex = _memoryList.Count - 1;
                double lastValue = _memoryList[lastIndex];
                _memoryList[lastIndex] = lastValue - currentValue;
                _memory = _memoryList[lastIndex];
            }
            else
            {
                _memory = -currentValue;
                _memoryList.Add(_memory);
            }
        }
    }
}
