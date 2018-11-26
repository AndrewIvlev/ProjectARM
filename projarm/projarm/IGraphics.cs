using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace projarm
{
    /*
     * Здесь был объявлен интерфейс IGraphics,
     * который предоставляет метод для рисования объекта.
     * Этот интерфейс может реализовать, например, класс Manipulator.
     * Классы Unit и Joint совсем разные сущности,
     * и они не имеют общего базового класса,
     * но мы можем создать список указателей на интерфейс IGraphics,
     * и работать с такими объектами, как с однотипными (с одинаковым интерфейсом).
     * Этот пример с IGraphics более наглядно отображает то, что нам дают интерфейсы.
     */

    interface IGraphics
    {
        void Show(Graphics gr);
        void Hide(Graphics gr);
        void Move(Graphics gr);
    }
}
