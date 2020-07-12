using System;

namespace GhepHinh
{
    [Serializable]
    public class SendObject
    {
        // Init Khởi tạo khi client kết nối
        // Select: Khi có một ai đó chọn mảnh khác
        // Translate: Khi có ai đó dịch chuyển một mảnh
        // Rotate: Khi xoay một mảnh
        // Append: Khi có mảnh bị đưa sang Main
        public const int INIT = 0;
        public const int SELECT_REMOTE = 1;
        public const int TRANSLATE_REMOTE = 2;
        public const int ROTATE_REMOTE = 3;
        public const int APPEND_MAIN = 4;
        public const int SELECT_MAIN = 5;
        public const int TRANSLATE_MAIN = 6;
        public const int ROTATE_MAIN = 7;
        public const int WIN = 8;
        public const int LOCK = 9;
        public const int UNLOCK = 10;

        public int type;
        public Object data;

        public SendObject(int type, Object data)
        {
            this.type = type;
            this.data = data;
        }
    }
}
