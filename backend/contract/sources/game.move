module games::player {
    use sui::coin::{Self, Coin};
    use sui::event;
    use sui::object::{Self, ID, UID};
    use sui::math;
    use sui::sui::SUI;
    use sui::transfer;
    use sui::tx_context::{Self, TxContext};
    use std::option::{Self, Option};

    struct Player has key, store {
        id: UID,
        power: u64,
        game_id: ID,
    }
    struct GameInfo has key {
        id: UID
    }

    /// Event emitted each time a Hero slays a Boar
    struct Attack has copy, drop {
        attaker_address: address,
        attaker: ID,
        prey: ID,
        game_id: ID,
    }
struct Coin has key, store {
        id: UID,
        game_id: ID,
    }
 

    #[allow(unused_function)]
    fun init(ctx: &mut TxContext) {
        create(ctx);
    }
    public entry fun new_game(ctx: &mut TxContext) {
        create(ctx);
    }

    /// Create a new game. Separated to bypass public entry vs init requirements.
    fun create(ctx: &mut TxContext) {
        let sender = tx_context::sender(ctx);
        let id = object::new(ctx);
        let game_id = object::uid_to_inner(&id);

        transfer::freeze_object(GameInfo {
            id,
            
        });
    }

    public entry fun attack(
        game: &GameInfo, attaker: &mut Player, prey: Player, ctx: &TxContext
    ) {
        check_id(game, attaker.game_id);
        check_id(game, prey.game_id);
        let Player { id: id, power: power,game_id: _ } = prey;
        let prey_power = prey.power;
        let player_power = attaker.power;
        while (player_power > prey_power) {
            player_power = player_power + prey_power;
            assert!(prey_power);
        };
        attacker.power = player_power;
        
       
        event::emit(Kill {
            attaker_address: tx_context::sender(ctx),
            attaker: object::uid_to_inner(&attaker.id),
            prey: object::uid_to_inner(&prey.id),
            game_id: id(game)
        });
        object::delete(prey.id);
    }

  
    public fun Collect_power(coin: &Coin):{
        player.power=player.power+1
    }
   

    public entry fun add_player(
        game: &GameInfo, payment: Coin<SUI>, ctx: &mut TxContext
    ) {
        let hero = create_player(game, sword, ctx);
        transfer::public_transfer(hero, tx_context::sender(ctx))
    }
    public fun create_player(
        game: &GameInfo, ctx: &mut TxContext
    ): Player {
        Player {
            id: object::new(ctx),
            power: 1,
            game_id: id(game)
        }
    }



    public fun check_id(game_info: &GameInfo, id: ID) {
        assert!(id(game_info) == id, 403); // TODO: error code
    }

    public fun id(game_info: &GameInfo): ID {
        object::id(game_info)
    }

  

}