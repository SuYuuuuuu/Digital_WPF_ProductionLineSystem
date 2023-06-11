

namespace WpfProductionLineApp.DobotCommunicationsFrameWork
{
    enum ptpMode
    {
        JUMP_XYZ, //JUMP 模式，（x,y,z,r）为笛卡尔坐标系下的目标点坐标
        MOVJ_XYZ, //MOVJ 模式，（x,y,z,r）为笛卡尔坐标系下的目标点坐标
        MOVL_XYZ, //MOVL 模式，（x,y,z,r）为笛卡尔坐标系下的目标点坐标
        JUMP_ANGLE, //JUMP 模式，（x,y,z,r）为关节坐标系下的目标点坐标
        MOVJ_ANGLE, //MOVJ 模式，（x,y,z,r）为关节坐标系下的目标点坐标
        MOVL_ANGLE, //MOVL 模式，（x,y,z,r）为关节坐标系下的目标点坐标
        MOVJ_INC, //MOVJ 模式，（x,y,z,r）为关节坐标系下的坐标增量
        MOVL_INC, //MOVL 模式，（x,y,z,r）为笛卡尔坐标系下的坐标增量
        MOVJ_XYZ_INC, //MOVJ 模式，（x,y,z,r）为笛卡尔坐标系下的坐标增量
        JUMP_MOVL_XYZ //JUMP 模式，平移时运动模式为 MOVL。（x,y,z,r）为笛卡尔坐标系下的坐标增量

    }

    struct jogCmd
    {
        public byte isJoint;//点动方式 0—坐标轴点动 1—关节点动
    }

    enum jogMode
    {
        IDEL, //无效状态
        AP_DOWN, // X+/Joint1+
        AN_DOWN, // X-/Joint1-
        BP_DOWN, // Y+/Joint2+
        BN_DOWN, // Y-/Joint2-
        CP_DOWN, // Z+/Joint3+
        CN_DOWN, // Z-/Joint3-
        DP_DOWN, // R+/Joint4+
        DN_DOWN // R-/Joint4-
    }

    enum Alarm
    {
        // Common error
        ERR_COMMON_MIN = 0x00,
        ERR_COMMON_RESET = ERR_COMMON_MIN,
        ERR_COMMON_MAX = 0x0f,
        // Plan error
        ERR_PLAN_MIN = 0x10,
        ERR_PLAN_INV_SINGULARITY = ERR_PLAN_MIN,
        ERR_PLAN_INV_CALC,
        ERR_PLAN_INV_LIMIT,
        ERR_PLAN_PUSH_DATA_REPEAT,
        ERR_PLAN_ARC_INPUT_PARAM,
        ERR_PLAN_JUMP_PARAM,
        ERR_PLAN_MAX = 0x1f,
        // Move error
        ERR_MOVE_MIN = 0x20,
        ERR_MOVE_INV_SINGULARITY = ERR_MOVE_MIN,
        ERR_MOVE_INV_CALC,
        ERR_MOVE_INV_LIMIT,
        ERR_MOVE_MAX = 0x2f,
        // Over speed error
        ERR_OVERSPEED_MIN = 0x30,
        ERR_OVERSPEED_AXIS1 = ERR_OVERSPEED_MIN,
        ERR_OVERSPEED_AXIS2,
        ERR_OVERSPEED_AXIS3,
        ERR_OVERSPEED_AXIS4,
        ERR_OVERSPEED_MAX = 0x3f,
        // Limit error
        ERR_LIMIT_MIN = 0x40,
        ERR_LIMIT_AXIS1_POS = ERR_LIMIT_MIN,
        ERR_LIMIT_AXIS1_NEG,
        ERR_LIMIT_AXIS2_POS,
        ERR_LIMIT_AXIS2_NEG,
        ERR_LIMIT_AXIS3_POS,
        ERR_LIMIT_AXIS3_NEG,
        ERR_LIMIT_AXIS4_POS,
        ERR_LIMIT_AXIS4_NEG,
        ERR_LIMIT_AXIS23_POS,
        ERR_LIMIT_AXIS23_NEG,
        //ERR_LIMIT_SINGULARITY,
        ERR_LIMIT_MAX = 0x4f,
        // Lose Step error
        ERR_LOSE_STEP_MIN = 0x50,
        ERR_LOSE_STEP_AXIS1 = ERR_LOSE_STEP_MIN,
        ERR_LOSE_STEP_AXIS2,
        ERR_LOSE_STEP_AXIS3,
        ERR_LOSE_STEP_AXIS4,
        ERR_LOSE_STEP_MAX = 0x5f,
    }
}
