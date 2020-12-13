#ifdef SIRE_DEBUG   // 判断是否debug模式，实际代码用其它方式实现

using std::string;  // C++风格，改成C不难。
using std::cout;
using std::endl;
using std::vector;
using std::set;

struct writeInfo
{
    string author;          // e.g.: "RK"
    string function;        // e.g.: "代码功能，用于光环修改"
};

const int TOTAL_BYTE_NUM = 10000000; // 改为实际程序内存byte地址

auto bitmap = vector<vector<writeInfo>>; // 全局的bitmap

void register(unsigned int address, unsigned int length, string author, string description) {   // 传入修改的首地址、持续长度、和一些作者信息之类的
    info = writeInfo(author, description);
    for (auto i = address; i < address + length; i++) {
        bitmap[i] = info;
    }
}

void check() {  // 在全部修改完成后调用，用于检测重复
    for (auto addr_info : bitmap) { // 遍历所有地址
        if (bitmap[address].size() > 1) {  // 已经被改过不止一次了
            cout << "[WARNING] repeated injection at address " << i;
            auto authors = set<string>;   // 所有修改过的（不同）作者
            auto functions = set<string>; // 同上，所有修改过的（不同）功能
            for (writeInfo : addr_info) {   // 遍历该地址的所有修改
                authors.insert(writeInfo[author]);
                functions.insert(writeInfo[function]);
            }
            cout << "[INFO] different authors: " << authors << "\ndifferent functions: " functions << endl;
        }
    }
}

#endif