using System;

// ở đây lưu sự kiện được gửi đi
// mỗi sự kiện sẽ gồm có các thông tin như kiểu (khởi tạo, dịch mảnh, xoay mảnh ...)
// và dữ liệu kèm theo, về sau khi nhận dữ liệu sẽ dùng switch case để xử lý

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
        // Lock, Unlock: Khóa ko cho dùng, khi có ai đó kéo dê chuột
        // Lưu ý một số sự kiện ko cần dữ liệu như Rotate, Append, Win, Lock, Unlock
        // do nó sẽ xoay, append luôn mảnh đang được chọn
        
        public const int INIT = 0;
        public const int SELECT_REMOTE = 1;
        public const int TRANSLATE_REMOTE = 2;
        public const int ROTATE_REMOTE = 3;
        public const int APPEND_MAIN = 4;
        public const int SELECT_MAIN = 5;
        public const int TRANSLATE_MAIN = 6;
        public const int ROTATE_MAIN = 7;
        public const int WIN = 8;
        public const int LOCK_REMOTE = 9;
        public const int UNLOCK_REMOTE = 10;
        public const int LOCK_MAIN = 11;
        public const int UNLOCK_MAIN = 12;

        public int type;
        public Object data;

        public SendObject(int type, Object data)
        {
            this.type = type;
            this.data = data;
        }
    }
}
